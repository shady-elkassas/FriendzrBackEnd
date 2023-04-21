

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Social.Entity.Models;
using System.Linq;

namespace Social.Entity.DBContext
{
    public class AuthDBContext : IdentityDbContext<User, ApplicationRole, string, IdentityUserClaim<string>, ApplicationUserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {

        public AuthDBContext(DbContextOptions<AuthDBContext> options) : base(options)
        {
            //this.Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<ApplicationUserRole> ApplicationUserRoles { get; set; }
        public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public DbSet<User> ApplicationUsers { get; set; }
        public DbSet<LoggedinUser> LoggedinUser { get; set; }
        public DbSet<UserDetails> UserDetails { get; set; }
        public DbSet<UserImage> UserImages { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<UserCodeCheck> UserCodeCheck { get; set; }
        public DbSet<DistanceFilterHistory> DistanceFilterHistory { get; set; }
        public DbSet<AppConfigration> AppConfigrations { get; set; }
        public DbSet<LinkAccount> LinkAccount { get; set; }
        public DbSet<BWErrorLog> BWErrorLog { get; set; }
        public DbSet<FilteringAccordingToAgeHistory> FilteringAccordingToAgeHistory { get; set; }
        public DbSet<ChatGroup> ChatGroups { get; set; }
        public DbSet<ChatGroupSubscribers> ChatGroupSubscribers { get; set; }
        public DbSet<DeletedUsersLog> DeletedUsersLogs { get; set; }
        public DbSet<DeletedEventLog> DeletedEventLogs { get; set; }
        public DbSet<ChatGroupReport> ChatGroupReports { get; set; }
        public DbSet<EventData> EventData { get; set; } // =>
        public DbSet<FavoriteEvent> FavoriteEvents { get; set; } // =>
        public DbSet<EventCategory> EventCategories { get; set; }
        public DbSet<listoftags> listoftags { get; set; }
        public DbSet<category> category { get; set; }
        //public DbSet<eventattend> eventattend { get; set; }
        public DbSet<Interests> Interests { get; set; }
        public DbSet<EventColor> EventColor { get; set; }
        public DbSet<Requestes> Requestes { get; set; }
        public DbSet<ReportReason> ReportReasons { get; set; }
        public DbSet<UserMessages> UserMessages { get; set; }
        public DbSet<UserReport> UserReports { get; set; }
        public DbSet<EventReport> EventReports { get; set; }
        public DbSet<Messagedata> Messagedata { get; set; } // =>>>
        public DbSet<FireBaseDatamodel> FireBaseDatamodel { get; set; }
        public DbSet<EventChatAttend> EventChatAttend { get; set; } //=>
        public DbSet<AppearanceTypes_UserDetails> AppearanceTypes_UserDetails { get; set; }
        public DbSet<WhatBestDescripsMeList> WhatBestDescripsMeList { get; set; }
        public DbSet<WhatBestDescripsMe> WhatBestDescripsMe { get; set; }
        public DbSet<Iprefertolist> Iprefertolist { get; set; }
        public DbSet<preferto> preferto { get; set; }
        public DbSet<EventTypeList> EventTypeList { get; set; }
        public DbSet<DeletedUser> DeletedUsers { get; set; }
        public DbSet<UserLinkClick> UserLinkClicks { get; set; }
        public DbSet<EventTracker> EventTrackers { get; set; }
        public DbSet<EventCategoryTracker> EventCategoryTrackers { get; set; }
        public DbSet<SkippedUser> SkippedUsers { get; set; }
        public DbSet<SkippedEvent> SkippedEvents { get; set; } //=>
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
       // public DbSet<EventDataTicketMaster> EventDataTicketMaster { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            //Set All Relation On Delete No Action
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>()
          .Property(b => b.RegistrationDate)
          .HasDefaultValueSql("getdate()");            
            modelBuilder.Entity<AppConfigration>()
          .Property(b => b.RegistrationDate)
          .HasDefaultValueSql("getdate()");

            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }

            modelBuilder.Entity<UserReport>()
      .HasOne(p => p.CreatedBy_User)
      .WithMany(t => t.UserReports_CreatedBy)
      .HasForeignKey(m => m.CreatedBy_UserID)
      .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserReport>()
 .HasOne(p => p.ReportedUser)
 .WithMany(t => t.UserReports_Reported)
 .HasForeignKey(m => m.UserID)
 .OnDelete(DeleteBehavior.Restrict);

            //            modelBuilder.Entity<Requestes>()
            //    .HasOne<UserDetails>(e => e.User).WithMany(d => d.Requestesto)
            //    .HasForeignKey(e => e.UserId).IsRequired(true)
            //    .OnDelete(DeleteBehavior.Cascade);
            //            modelBuilder.Entity<Requestes>()
            //   .HasOne<UserDetails>(e => e.UserRequest).WithMany(d => d.Requestesfor)
            //   .HasForeignKey(e => e.UserRequestId).IsRequired(true)
            //   .OnDelete(DeleteBehavior.Cascade);
            //            modelBuilder.Entity<Requestes>()
            //   .HasOne<UserDetails>(e => e.Userblock).WithMany(d => d.Requestesblock)
            //   .HasForeignKey(e => e.UserblockId).IsRequired(true)
            //   .OnDelete(DeleteBehavior.Cascade);

            //            modelBuilder.Entity<eventattend>()
            // .HasOne<UserDetails>(e => e.Userattend).WithMany(d => d.eventattend)
            // .HasForeignKey(e => e.UserattendId).IsRequired(true)
            // .OnDelete(DeleteBehavior.Cascade);
            //            modelBuilder.Entity<EventData>()
            //.HasOne<UserDetails>(e => e.User).WithMany(d => d.EventData)
            //.HasForeignKey(e => e.UserId).IsRequired(true)
            //.OnDelete(DeleteBehavior.Cascade);
            //            modelBuilder.Entity<LinkAccount>()
            //.HasOne<UserDetails>(e => e.User).WithMany(d => d.LinkAccount)
            //.HasForeignKey(e => e.UserId).IsRequired(true)
            //.OnDelete(DeleteBehavior.Cascade);
            //            modelBuilder.Entity<listoftags>()
            //.HasOne<UserDetails>(e => e.User).WithMany(d => d.listoftags)
            //.HasForeignKey(e => e.UserId).IsRequired(true)
            //.OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Requestes>()
                .HasOne(p => p.User)
                .WithMany(t => t.Requestesto)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Requestes>()
                .HasOne(p => p.UserRequest)
                .WithMany(t => t.Requestesfor)
                .HasForeignKey(m => m.UserRequestId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Requestes>()
                .HasOne(p => p.Userblock)
                .WithMany(t => t.Requestesblock)
                .HasForeignKey(m => m.UserblockId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserMessages>()
               .HasOne(p => p.ToUser)
               .WithMany(t => t.UserMessagesto)
               .HasForeignKey(m => m.ToUserId)
               .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<UserMessages>()
                .HasOne(p => p.User)
                .WithMany(t => t.UserMessages)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            //Uniq Keys 
            //modelBuilder.Entity<eventattend>().HasIndex(a => new { a.EventDataid, a.UserattendId }).IsUnique();
            modelBuilder.Entity<City>().HasIndex(e => e.GoogleName).IsUnique();
            modelBuilder.Entity<Country>().HasIndex(e => e.GoogleName).IsUnique();
            modelBuilder.Entity<EventChatAttend>().HasIndex(a => new { a.EventDataid, a.UserattendId }).IsUnique();
            modelBuilder.Entity<EventChatAttend>(b =>
            {               
                b.HasOne(ur => ur.EventData).WithMany(u => u.EventChatAttend).HasForeignKey(b => b.EventDataid).OnDelete(DeleteBehavior.Cascade).IsRequired();
              //  b.HasOne(ur => ur.EventDataTicketMaster).WithMany(u => u.EventChatAttend).HasForeignKey(b => b.TicketMasterEventDataid).OnDelete(DeleteBehavior.NoAction).IsRequired();
                //b.HasOne(ur => ur.Userattend).WithOne(u=>u.EventChatAttend)
                //.HasForeignKey<EventChatAttend>(l => l.UserattendId)
                //.OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ChatGroupSubscribers>().HasIndex(a => new { a.ChatGroupID, a.UserID }).IsUnique();
            modelBuilder.Entity<ChatGroupSubscribers>(b =>
            {
                b.HasOne(c => c.ChatGroup).WithMany(cs => cs.Subscribers).HasForeignKey(b => b.ChatGroupID).OnDelete(DeleteBehavior.Cascade).IsRequired();
            });

            modelBuilder.Entity<ApplicationUserRole>(ur =>
            {
                ur.HasKey(ur => new { ur.UserId, ur.RoleId });
                ur.HasOne(ur => ur.User).WithMany(u => u.UserRoles).HasForeignKey(ur => ur.UserId).IsRequired();
                ur.HasOne(ur => ur.Role).WithMany(r => r.UserRoles).HasForeignKey(ur => ur.RoleId).IsRequired();

            });

            modelBuilder.Entity<UserLinkClick>(ur =>
            {
                ur.HasOne(ur => ur.userDetails).WithMany(u => u.UserLinkClicks).HasForeignKey(ur => ur.UserId).IsRequired();

            });

            modelBuilder.Entity<EventTracker>(b =>
            {
                b.HasOne(ur => ur.Event).WithMany(u => u.EventTrackers).HasForeignKey(b => b.EventId).IsRequired();
                b.HasOne(ur => ur.User).WithMany(u => u.EventTrackers).HasForeignKey(b => b.UserId).IsRequired();

            });

            modelBuilder.Entity<EventCategoryTracker>(b =>
            {
                b.HasOne(ur => ur.Category).WithMany(u => u.EventCategoryTrackers).HasForeignKey(b => b.CategoryId).IsRequired();
                b.HasOne(ur => ur.User).WithMany(u => u.EventCategoryTrackers).HasForeignKey(b => b.UserId).IsRequired();

            });

            modelBuilder.Entity<SkippedUser>(b =>
            {
                b.HasOne(ur => ur.UserDetail).WithMany(u => u.SkippedUsers).HasForeignKey(b => b.UserId).OnDelete(DeleteBehavior.Cascade).IsRequired();
                b.HasOne(ur => ur.SkippedUserDetail).WithMany(u => u.UserSkippedUsers).HasForeignKey(b => b.SkippedUserId).OnDelete(DeleteBehavior.NoAction).IsRequired();

            });

            modelBuilder.Entity<SkippedEvent>(b =>
            {
                b.HasOne(ur => ur.UserDetail).WithMany(u => u.SkippedEvents).HasForeignKey(b => b.UserId).IsRequired();
                b.HasOne(ur => ur.EventData).WithMany(u => u.SkippedEvents).HasForeignKey(b => b.EventId).OnDelete(DeleteBehavior.Cascade).IsRequired();
            });
            modelBuilder.Entity<Messagedata>(b =>
            {               
               // b.HasOne(ur => ur.EventData).WithMany(u => u.Messagedata).HasForeignKey(b => b.EventDataid).OnDelete(DeleteBehavior.Cascade);
                b.HasOne(ur => ur.User).WithMany(u => u.Messagedata).HasForeignKey(b => b.UserId).OnDelete(DeleteBehavior.Cascade).IsRequired();
            });

            modelBuilder.Entity<UserDetails>(b =>
            {
                b.Property(ur => ur.IsWhiteLabel).HasDefaultValue(false);
            });
            modelBuilder.Entity<UserDetails>(b =>
            {
                b.Property(ur => ur.ImageIsVerified).HasDefaultValue(false);
            });
            modelBuilder.Entity<EventData>(b =>
            {
                b.HasOne(ur => ur.User).WithMany(u => u.EventData).HasForeignKey(b => b.UserId).OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<UserImage>(b =>
            {
                b.HasOne(ur => ur.UserDetails).WithMany(u => u.UserImages).HasForeignKey(b => b.UserDetailsId).OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<FireBaseDatamodel>(b =>
            {
                b.HasOne(ur => ur.User).WithMany(u => u.FireBaseDatamodel).HasForeignKey(b => b.userid).OnDelete(DeleteBehavior.Cascade).IsRequired();
            });


            //modelBuilder.Entity<EventDataTicketMaster>(b =>
            //{
            //    b.HasKey(x => x.Id);
            //   // b.Property(x => x.Id).UseIdentityColumn();
            //    b.HasOne(ur => ur.User).WithMany(u => u.EventDataTicketMaster).HasForeignKey(b => b.UserId).OnDelete(DeleteBehavior.Cascade);
            //    b.HasMany(r => r.Messagedata).WithOne(x => x.EventDataTicketMaster).HasForeignKey(b => b.TicketMasterEventDataid).OnDelete(DeleteBehavior.Cascade).IsRequired();
            //    b.HasMany(r => r.EventReports).WithOne(x => x.EventDataTicketMaster).HasForeignKey(b => b.TicketMasterEventDataID).OnDelete(DeleteBehavior.Cascade).IsRequired();
            //    b.HasOne(r => r.EventTypeList).WithMany(x => x.EventDataTicketMaster).HasForeignKey(b => b.EventTypeListid).OnDelete(DeleteBehavior.Cascade).IsRequired();
            //    b.HasOne(r => r.City).WithMany(x => x.EventDataTicketMaster).HasForeignKey(b => b.CityID).OnDelete(DeleteBehavior.Cascade).IsRequired();
            //    b.HasOne(r => r.Country).WithMany(x => x.EventDataTicketMaster).HasForeignKey(b => b.CountryID).OnDelete(DeleteBehavior.Cascade).IsRequired();
            //    b.HasMany(r => r.EventTrackers).WithOne(x => x.EventDataTicketMaster).HasForeignKey(b => b.TicketMasetrEventId).OnDelete(DeleteBehavior.Cascade).IsRequired();
            //    b.HasMany(r => r.SkippedEvents).WithOne(x => x.EventDataTicketMaster).HasForeignKey(b => b.TicketMasterEventId).OnDelete(DeleteBehavior.Cascade).IsRequired();
            //});

        }
    }
}
