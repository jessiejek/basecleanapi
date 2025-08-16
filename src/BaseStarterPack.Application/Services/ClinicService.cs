using BaseStarterPack.Application.Interfaces.Common;
using BaseStarterPack.Domain.Entities;

namespace BaseStarterPack.Application.Services;

public class ClinicService(IUnitOfWork uow)
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
}
