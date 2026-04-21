using BaseStarterPack.Application.Interfaces.Common;
using BaseStarterPack.Domain.Entities;

namespace BaseStarterPack.Application.Services;

public class ClinicService(IUnitOfWork uow, ISqlDataAccess sqlDataAccess)
{
    public async Task<IEnumerable<Clinic>> GetAllAsync(CancellationToken ct = default)
        => await uow.Clinics.GetAllAsync(cancellationToken: ct);

    public async Task<Clinic?> GetByIdAsync(int clinicId, CancellationToken ct = default)
        => await uow.Clinics.GetAsync(x => x.ClinicId == clinicId, cancellationToken: ct);

    public async Task<int> CreateAsync(Clinic clinic, CancellationToken ct = default)
    {
        await uow.Clinics.AddAsync(clinic, ct);
        await uow.SaveChangesAsync(ct);
        return 1;
    }

    public async Task<bool> UpdateAsync(Clinic clinic, CancellationToken ct = default)
    {
        uow.Clinics.Update(clinic);
        await uow.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int clinicId, CancellationToken ct = default)
    {
        var entity = await uow.Clinics.GetAsync(x => x.ClinicId == clinicId, cancellationToken: ct);
        if (entity is null) return false;
        uow.Clinics.Remove(entity);
        await uow.SaveChangesAsync(ct);
        return true;
    }

    // Example: reporting/raw SQL query on a named connection (e.g., ReportingDb)
    public async Task<int> GetClinicCountFromReportingAsync(CancellationToken ct = default)
    {
        const string sql = "SELECT COUNT(1) FROM Clinics;";
        var count = await sqlDataAccess.LoadFirstAsync<int, dynamic>(
            sql,
            new { },
            connectionName: "ReportingDb",
            isStoredProcedure: false);

        return count;
    }

    // Example: stored procedure call via SQL data access abstraction
    public async Task<Clinic?> GetClinicByStoredProcAsync(int clinicId, CancellationToken ct = default)
    {
        const string storedProc = "usp_Clinics_GetById";
        return await sqlDataAccess.LoadFirstAsync<Clinic, dynamic>(
            storedProc,
            new { ClinicId = clinicId },
            connectionName: "DefaultConnection",
            isStoredProcedure: true);
    }
}
