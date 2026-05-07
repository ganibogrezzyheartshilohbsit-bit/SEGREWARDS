namespace SEGREWARDS_PROJECT.Services
{
    public interface IRewardRedemptionService
    {
        RedeemResult Redeem(long userId, short rewardCatalogId);
    }
}

