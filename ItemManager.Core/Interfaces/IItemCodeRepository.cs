namespace ItemManager.Core.Interfaces
{
    public interface IItemCodeRepository
    {
        Task<int> GetNextSequenceAsync(
            int itemTypeId,
            int? itemSubTypeId);

        Task<bool> IsCodeUniqueAsync(string itemCode);

    }
}
