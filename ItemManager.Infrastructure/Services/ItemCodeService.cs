using ItemManager.Core.Interfaces;

namespace ItemManager.Infrastructure.Services
{
    public class ItemCodeService : IItemCodeService
    {
        private readonly IItemCodeRepository _itemCodeRepo;

        public ItemCodeService(IItemCodeRepository itemCodeRepo)
        {
            _itemCodeRepo = itemCodeRepo;
        }

        public async Task<string> GenerateCodeAsync(
            int itemTypeId,
            int? itemSubTypeId,
            string itemTypeName,
            string? itemSubTypeName)
        {
            string typeAcronym = itemTypeName
                .Substring(0, Math.Min(2, itemTypeName.Length))
                .ToUpper();

            string? subTypeAcronym = itemSubTypeName != null
                ? itemSubTypeName
                    .Substring(0, Math.Min(2, itemSubTypeName.Length))
                    .ToUpper()
                : null;

            int seq = await _itemCodeRepo
                .GetNextSequenceAsync(itemTypeId, itemSubTypeId);

            string seqStr = seq.ToString("D3");

            if (subTypeAcronym != null)
            {
                return $"{typeAcronym}-{subTypeAcronym}-{seqStr}";
            }

            return $"{typeAcronym}-{seqStr}";
        }

        public async Task<bool> IsCodeUniqueAsync(string itemCode)
        {
            return await _itemCodeRepo.IsCodeUniqueAsync(itemCode);
        }

    }
}
