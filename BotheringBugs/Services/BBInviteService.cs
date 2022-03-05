using BotheringBugs.Data;
using BotheringBugs.Models;
using BotheringBugs.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BotheringBugs.Services
{
    public class BBInviteService : IBBInviteService
    {
        private ApplicationDbContext _context;

        public BBInviteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public ApplicationDbContext Context { get; }

        public async Task<bool> AcceptInviteAsync(Guid? token, string userId, int companyId)
        {
            Invite invite = await _context.Invites.FirstOrDefaultAsync(i => i.CompanyToken == token);

            if (invite == null)
            {
                return false;
            }

            try
            {
                invite.IsValid = false;
                invite.InviteeId = userId;
                await _context.SaveChangesAsync();

                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error - error accepting invite: {ex.Message}");
                throw;
            }
        }

        public async Task AddNewInviteAsync(Invite invite)
        {
            try
            {
                await _context.Invites.AddAsync(invite);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error - error adding invite: {ex.Message}");

                throw;
            }
        }

        public async Task<bool> AnyInviteAsync(Guid token, string email, int companyId)
        {
            try
            {
                bool result = await _context.Invites.Where(i => i.CompanyId == companyId)
                                                    .AnyAsync(i => i.CompanyToken == token && i.InviteeEmail == email);

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error - error finding invite: {ex.Message}");

                throw;
            }
        }

        public async Task<Invite> GetInviteAsync(int inviteId, int companyId)
        {
            try
            {
                Invite invite = await _context.Invites.Where(i => i.CompanyId == companyId)
                                                        .Include(i => i.Company)
                                                        .Include(i => i.Project)
                                                        .Include(i => i.Invitor)
                                                        .FirstOrDefaultAsync(i => i.Id == inviteId);

                return invite;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error - error finding the invite: {ex.Message}");

                throw;
            }
        }

        public async Task<Invite> GetInviteAsync(Guid token, string email, int companyId)
        {
            try
            {
                Invite invite = await _context.Invites.Where(i => i.CompanyId == companyId)
                                                    .Include(i => i.Company)
                                                    .Include(i => i.Project)
                                                    .Include(i => i.Invitor)
                                                    .FirstOrDefaultAsync(i => i.CompanyToken == token && i.InviteeEmail == email);

                return invite;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error - error finding the invite: {ex.Message}");

                throw;
            }
        }

        public async Task<bool> ValidateInviteCodeAsync(Guid? token)
        {
            if(token == null)
            {
                return false;
            }

            bool result = false;

            Invite invite = await _context.Invites.FirstOrDefaultAsync(i => i.CompanyToken != token);

            if(invite != null)
            {
                //determine invite date
                DateTime inviteDate = invite.InviteDate.DateTime;

                //Custom validation of invite based on the date it was issues
                //In this case we are allowing an invite to be vaild of 7 days
                bool validate = (DateTime.Now - inviteDate).TotalDays <= 7;

                if (validate)
                {
                    result = invite.IsValid;
                }

            }

            return result;
        }
    }
}
