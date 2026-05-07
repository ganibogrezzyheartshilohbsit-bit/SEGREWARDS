-- SEGREWARDS / Smart Waste — MySQL schema
-- Aligns with WinForms UI (users, waste submissions, EcoPoints, rewards, audit)
-- Reference concepts: https://github.com/diaskalana/smart-waste-management-system (users, reports, rewards, transactions)

CREATE DATABASE IF NOT EXISTS segrewards
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;

USE segrewards;

SET NAMES utf8mb4;

-- ---------------------------------------------------------------------------
-- Roles (extensible; default student)
-- ---------------------------------------------------------------------------
CREATE TABLE roles (
  id TINYINT UNSIGNED NOT NULL AUTO_INCREMENT,
  code VARCHAR(32) NOT NULL,
  name VARCHAR(64) NOT NULL,
  PRIMARY KEY (id),
  UNIQUE KEY uk_roles_code (code)
) ENGINE=InnoDB;

INSERT IGNORE INTO roles (id, code, name) VALUES
  (1, 'student', 'Student'),
  (2, 'staff', 'Staff'),
  (3, 'admin', 'Administrator');

-- ---------------------------------------------------------------------------
-- Users (login uses student_number; optional email from signup form)
-- ---------------------------------------------------------------------------
CREATE TABLE users (
  id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  student_number VARCHAR(64) NOT NULL,
  email VARCHAR(255) NULL,
  password_hash VARBINARY(64) NOT NULL,
  password_salt VARBINARY(32) NOT NULL,
  full_name VARCHAR(255) NOT NULL,
  year_level VARCHAR(64) NULL,
  course VARCHAR(512) NULL,
  role_id TINYINT UNSIGNED NOT NULL DEFAULT 1,
  eco_points_balance INT NOT NULL DEFAULT 0,
  is_active TINYINT(1) NOT NULL DEFAULT 1,
  created_at DATETIME(3) NOT NULL DEFAULT CURRENT_TIMESTAMP(3),
  updated_at DATETIME(3) NOT NULL DEFAULT CURRENT_TIMESTAMP(3) ON UPDATE CURRENT_TIMESTAMP(3),
  PRIMARY KEY (id),
  UNIQUE KEY uk_users_student_number (student_number),
  UNIQUE KEY uk_users_email (email),
  KEY idx_users_role (role_id),
  CONSTRAINT fk_users_role FOREIGN KEY (role_id) REFERENCES roles (id)
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------------
-- Waste types (maps to Form5: Bottle / Paper / Biodegradable)
-- ---------------------------------------------------------------------------
CREATE TABLE waste_types (
  id SMALLINT UNSIGNED NOT NULL AUTO_INCREMENT,
  code VARCHAR(32) NOT NULL,
  display_name VARCHAR(128) NOT NULL,
  default_points INT NOT NULL,
  PRIMARY KEY (id),
  UNIQUE KEY uk_waste_types_code (code)
) ENGINE=InnoDB;

INSERT IGNORE INTO waste_types (id, code, display_name, default_points) VALUES
  (1, 'BOTTLE', 'Bottle', 10),
  (2, 'PAPER', 'Paper', 5),
  (3, 'BIO', 'Biodegradable', 3);

-- ---------------------------------------------------------------------------
-- Waste submissions (reports / collection workflow; staff can approve)
-- ---------------------------------------------------------------------------
CREATE TABLE waste_submissions (
  id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  user_id BIGINT UNSIGNED NOT NULL,
  waste_type_id SMALLINT UNSIGNED NOT NULL,
  student_number_snapshot VARCHAR(64) NOT NULL,
  full_name_snapshot VARCHAR(255) NOT NULL,
  year_level_snapshot VARCHAR(64) NULL,
  course_snapshot VARCHAR(512) NULL,
  proof_image MEDIUMBLOB NULL,
  proof_mime_type VARCHAR(128) NULL,
  proof_original_name VARCHAR(255) NULL,
  status VARCHAR(32) NOT NULL DEFAULT 'pending',
  points_awarded INT NOT NULL DEFAULT 0,
  reviewed_by_user_id BIGINT UNSIGNED NULL,
  reviewed_at DATETIME(3) NULL,
  staff_notes VARCHAR(512) NULL,
  created_at DATETIME(3) NOT NULL DEFAULT CURRENT_TIMESTAMP(3),
  PRIMARY KEY (id),
  KEY idx_ws_user_created (user_id, created_at),
  KEY idx_ws_status (status),
  KEY idx_ws_waste_type (waste_type_id),
  CONSTRAINT fk_ws_user FOREIGN KEY (user_id) REFERENCES users (id),
  CONSTRAINT fk_ws_waste_type FOREIGN KEY (waste_type_id) REFERENCES waste_types (id),
  CONSTRAINT fk_ws_reviewer FOREIGN KEY (reviewed_by_user_id) REFERENCES users (id)
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------------
-- EcoPoint ledger (earned / redeemed / adjustment) — reporting-friendly
-- ---------------------------------------------------------------------------
CREATE TABLE eco_point_transactions (
  id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  user_id BIGINT UNSIGNED NOT NULL,
  submission_id BIGINT UNSIGNED NULL,
  redemption_id BIGINT UNSIGNED NULL,
  type VARCHAR(16) NOT NULL,
  amount INT NOT NULL,
  description VARCHAR(512) NOT NULL,
  created_at DATETIME(3) NOT NULL DEFAULT CURRENT_TIMESTAMP(3),
  PRIMARY KEY (id),
  KEY idx_ept_user_created (user_id, created_at),
  KEY idx_ept_type (type),
  CONSTRAINT fk_ept_user FOREIGN KEY (user_id) REFERENCES users (id),
  CONSTRAINT fk_ept_submission FOREIGN KEY (submission_id) REFERENCES waste_submissions (id)
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------------
-- Reward catalog (search / redeem UI)
-- ---------------------------------------------------------------------------
CREATE TABLE reward_catalog (
  id SMALLINT UNSIGNED NOT NULL AUTO_INCREMENT,
  name VARCHAR(255) NOT NULL,
  description TEXT NULL,
  points_cost INT NOT NULL,
  image_path VARCHAR(512) NULL,
  is_active TINYINT(1) NOT NULL DEFAULT 1,
  sort_order INT NOT NULL DEFAULT 0,
  PRIMARY KEY (id),
  UNIQUE KEY uk_reward_catalog_name (name),
  KEY idx_reward_catalog_active (is_active, sort_order)
) ENGINE=InnoDB;

INSERT INTO reward_catalog (name, description, points_cost, sort_order) VALUES
  ('Recycle Tote Bag', 'Carry good habits everywhere', 1000, 1),
  ('Eco Water Bottle', 'Sustainable and eco friendly', 500, 2),
  ('School Supplies', 'Eco-friendly school supplies', 100, 3),
  ('Sweets Snacks', 'Sweet and salty small snacks', 50, 4)
ON DUPLICATE KEY UPDATE description = VALUES(description), points_cost = VALUES(points_cost);

-- ---------------------------------------------------------------------------
-- Redemptions (must be created before FK from eco_point_transactions in strict mode;
-- add FK after redemptions table exists — MySQL allows forward ref if we add FK later)
-- ---------------------------------------------------------------------------
CREATE TABLE redemptions (
  id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  user_id BIGINT UNSIGNED NOT NULL,
  reward_catalog_id SMALLINT UNSIGNED NOT NULL,
  points_spent INT NOT NULL,
  status VARCHAR(32) NOT NULL DEFAULT 'completed',
  created_at DATETIME(3) NOT NULL DEFAULT CURRENT_TIMESTAMP(3),
  PRIMARY KEY (id),
  KEY idx_redemptions_user (user_id, created_at),
  CONSTRAINT fk_redemptions_user FOREIGN KEY (user_id) REFERENCES users (id),
  CONSTRAINT fk_redemptions_reward FOREIGN KEY (reward_catalog_id) REFERENCES reward_catalog (id)
) ENGINE=InnoDB;

ALTER TABLE eco_point_transactions
  ADD CONSTRAINT fk_ept_redemption FOREIGN KEY (redemption_id) REFERENCES redemptions (id);

-- ---------------------------------------------------------------------------
-- Audit log
-- ---------------------------------------------------------------------------
CREATE TABLE audit_log (
  id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  user_id BIGINT UNSIGNED NULL,
  action VARCHAR(64) NOT NULL,
  entity_type VARCHAR(64) NULL,
  entity_id VARCHAR(64) NULL,
  details TEXT NULL,
  created_at DATETIME(3) NOT NULL DEFAULT CURRENT_TIMESTAMP(3),
  PRIMARY KEY (id),
  KEY idx_audit_created (created_at),
  CONSTRAINT fk_audit_user FOREIGN KEY (user_id) REFERENCES users (id)
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------------
-- Leaderboard view (optional; reporting queries can use this)
-- ---------------------------------------------------------------------------
DROP VIEW IF EXISTS v_leaderboard;
CREATE VIEW v_leaderboard AS
SELECT
  u.id AS user_id,
  u.student_number,
  u.full_name,
  u.eco_points_balance,
  COUNT(ws.id) AS submission_count,
  SUM(CASE WHEN ws.status = 'approved' THEN 1 ELSE 0 END) AS approved_submissions
FROM users u
LEFT JOIN waste_submissions ws ON ws.user_id = u.id
WHERE u.is_active = 1
GROUP BY u.id, u.student_number, u.full_name, u.eco_points_balance;
