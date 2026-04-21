using Gdn.Domain.Data.Repositories;

namespace Gdn.Web.Api.Vs.Features.Interventions;

internal static class InterventionValidation
{
    public static async Task<Error?> ValidateRowReferencesAsync(IEnumerable<int?> measurementUnitIds, IEnumerable<int?> taxRateIds, IMeasurementUnitRepository measurementUnitRepository, ITaxRateRepository taxRateRepository)
    {
        var measurementUnitIdSet = measurementUnitIds
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();

        if (measurementUnitIdSet.Count > 0)
        {
            var measurementUnits = await measurementUnitRepository.GetAllAsync(mu => measurementUnitIdSet.Contains(mu.Id));
            var foundMeasurementUnitIds = measurementUnits.Select(mu => mu.Id).ToHashSet();
            var invalidMeasurementUnitId = measurementUnitIdSet.FirstOrDefault(id => !foundMeasurementUnitIds.Contains(id));
            if (invalidMeasurementUnitId != 0)
                return InterventionErrors.InvalidMeasurementUnit(invalidMeasurementUnitId);
        }

        var taxRateIdSet = taxRateIds
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();

        if (taxRateIdSet.Count > 0)
        {
            var taxRates = await taxRateRepository.GetAllAsync(tr => taxRateIdSet.Contains(tr.Id));
            var foundTaxRateIds = taxRates.Select(tr => tr.Id).ToHashSet();
            var invalidTaxRateId = taxRateIdSet.FirstOrDefault(id => !foundTaxRateIds.Contains(id));
            if (invalidTaxRateId != 0)
                return InterventionErrors.InvalidTaxRate(invalidTaxRateId);
        }

        return null;
    }
}
