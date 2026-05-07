namespace SEGREWARDS_PROJECT.Services
{
    public sealed class RedeemResult
    {
        private RedeemResult(bool success, string message, int newBalance)
        {
            Success = success;
            Message = message;
            NewBalance = newBalance;
        }

        public bool Success { get; }
        public string Message { get; }
        public int NewBalance { get; }

        public static RedeemResult Ok(int newBalance)
        {
            return new RedeemResult(true, null, newBalance);
        }

        public static RedeemResult Fail(string message)
        {
            return new RedeemResult(false, message, 0);
        }
    }
}

