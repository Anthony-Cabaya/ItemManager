namespace ItemManager.Core.Interfaces
{
    public interface IItemCodeService
    {
        Task<string> GenerateCodeAsync(
            int itemTypeId,
            int? itemSubTypeId,
            string itemTypeName,
            string? itemSubTypeName);

        Task<bool> IsCodeUniqueAsync(string itemCode);

        Task<string> PreviewCodeAsync(
            int itemTypeId, int? itemSubTypeId,
            string itemTypeName, string? itemSubTypeName);

    }
}
