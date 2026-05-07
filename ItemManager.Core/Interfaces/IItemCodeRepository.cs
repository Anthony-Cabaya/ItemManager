namespace ItemManager.Core.Interfaces
{
    public interface IItemCodeRepository
    {
        Task<int> GetNextSequenceAsync(
            int itemTypeId,
            int? itemSubTypeId);

        Task<bool> IsCodeUniqueAsync(string itemCode);

        Task<int> PeekNextSequenceAsync(int itemTypeId, int? itemSubTypeId);

    }
}
