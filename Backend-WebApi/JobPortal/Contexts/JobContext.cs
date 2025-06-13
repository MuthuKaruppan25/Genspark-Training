using JobPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Contexts;

public class JobContext : DbContext
{
    public JobContext(DbContextOptions<JobContext> options) : base(options)
    {

    }
    public DbSet<User> users { get; set; }
    public DbSet<Recruiter> recruiters { get; set; }
    public DbSet<Company> companies { get; set; }
    public DbSet<Address> address { get; set; }
    public DbSet<IndustryType> industryTypes { get; set; }
    public DbSet<JobPost> jobPosts { get; set; }
    public DbSet<JobApplication> jobApplications { get; set; }
    public DbSet<PostSkills> postSkills { get; set; }
    public DbSet<Requirements> requirements { get; set; }
    public DbSet<Responsibilities> responsibilities { get; set; }
    public DbSet<Seeker> seekers { get; set; }
    public DbSet<SeekerSkills> seekerSkills { get; set; }
    public DbSet<Skill> skills { get; set; }
    public DbSet<FileModel> fileModels { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //User
        modelBuilder.Entity<User>().HasKey(u => u.guid)
                                    .HasName("PK_User");

        //Recruiter
        modelBuilder.Entity<Recruiter>().HasKey(u => u.guid)
                                    .HasName("PK_Recruiter");

        modelBuilder.Entity<Recruiter>()
                                    .HasOne(r => r.user)
                                    .WithOne(u => u.recruiter)
                                    .HasForeignKey<Recruiter>(r => r.UserId)
                                    .HasConstraintName("FK_Recruiter_UserId")
                                    .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Recruiter>()
                                    .HasOne(c => c.company)
                                    .WithMany(r => r.recruiters)
                                    .HasForeignKey(cr => cr.CompanyId)
                                    .HasConstraintName("FK_Recruiter_CompanyId")
                                    .OnDelete(DeleteBehavior.Restrict);

        //Company
        modelBuilder.Entity<Company>().HasKey(u => u.guid)
                                    .HasName("PK_Company");

        modelBuilder.Entity<Company>()
                                    .HasOne(c => c.industryType)
                                    .WithMany(i => i.companies)
                                    .HasForeignKey(c => c.IndustryTypeId)
                                    .OnDelete(DeleteBehavior.Restrict);

        //Address
        modelBuilder.Entity<Address>()
                                    .HasKey(a => a.guid)
                                    .HasName("PK_Address");
        modelBuilder.Entity<Address>()
                                    .HasOne(c => c.company)
                                    .WithMany(r => r.locations)
                                    .HasForeignKey(cr => cr.companyId)
                                    .HasConstraintName("FK_Address_CompanyId")
                                    .OnDelete(DeleteBehavior.Restrict);

        //Requirements

        modelBuilder.Entity<Requirements>()
                                    .HasKey(a => a.guid)
                                    .HasName("PK_Requirements");
        modelBuilder.Entity<Requirements>()
                                    .HasOne(r => r.jobPost)
                                    .WithMany(j => j.requirements)
                                    .HasForeignKey(r => r.PostId)
                                    .HasConstraintName("FK_Requirements_PostId")
                                    .OnDelete(DeleteBehavior.Restrict);

        //Responsibilities
        modelBuilder.Entity<Responsibilities>()
                                    .HasKey(a => a.guid)
                                    .HasName("PK_Responsibilities");
        modelBuilder.Entity<Responsibilities>()
                                    .HasOne(r => r.jobPost)
                                    .WithMany(j => j.responsibilities)
                                    .HasForeignKey(r => r.PostId)
                                    .HasConstraintName("FK_Requirements_PostId")
                                    .OnDelete(DeleteBehavior.Restrict);

        //Post Skills

        modelBuilder.Entity<PostSkills>()
                                    .HasKey(p => p.guid)
                                    .HasName("PK_PostSkill");

        modelBuilder.Entity<PostSkills>()
                                    .HasOne(ps => ps.JobPost)
                                    .WithMany(j => j.requiredSkills)
                                    .HasForeignKey(ps => ps.JobPostId)
                                    .HasConstraintName("FK_PostSkills_PostId")
                                    .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<PostSkills>()
                                    .HasOne(ps => ps.Skill)
                                    .WithMany(s => s.postSkills)
                                    .HasForeignKey(ps => ps.SkillId)
                                    .HasConstraintName("FK_PostSkills_SkillId");

        //IndustryType
        modelBuilder.Entity<IndustryType>()
                                        .HasKey(i => i.guid)
                                        .HasName("PK_IndustryType");

        //Seeker Skills

        modelBuilder.Entity<SeekerSkills>()
                                    .HasKey(p => p.guid)
                                    .HasName("PK_Seekerkill");

        modelBuilder.Entity<SeekerSkills>()
                                    .HasOne(ps => ps.seeker)
                                    .WithMany(j => j.seekerSkills)
                                    .HasForeignKey(ps => ps.SeekerId)
                                    .HasConstraintName("FK_SeekerSkills_PostId")
                                    .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<SeekerSkills>()
                                    .HasOne(ps => ps.skill)
                                    .WithMany(s => s.seekerSkills)
                                    .HasForeignKey(ps => ps.SkillId)
                                    .HasConstraintName("FK_SeekerSkills_SkillId");

        //JobApplicants

        modelBuilder.Entity<JobApplication>()
                                    .HasKey(p => p.guid)
                                    .HasName("PK_JobApplication");

        modelBuilder.Entity<JobApplication>()
                                    .HasOne(ps => ps.seeker)
                                    .WithMany(j => j.jobApplications)
                                    .HasForeignKey(ps => ps.SeekerId)
                                    .HasConstraintName("FK_JobApplications_SeekerId")
                                    .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<JobApplication>()
                                    .HasOne(ps => ps.jobPost)
                                    .WithMany(s => s.jobApplications)
                                    .HasForeignKey(ps => ps.JobPostId)
                                    .HasConstraintName("FK_JobApplications_PostId");

        //JobPosts
        modelBuilder.Entity<JobPost>()
                                    .HasKey(j => j.guid)
                                    .HasName("PK_JobPost");
        modelBuilder.Entity<JobPost>()
                                    .HasOne(u => u.recruiter)
                                    .WithMany(p => p.jobPosts)
                                    .HasForeignKey(up => up.RecruiterID)
                                    .HasConstraintName("FK_JobPost_RecruiterID")
                                    .OnDelete(DeleteBehavior.Restrict);

        //Seeker

        modelBuilder.Entity<Seeker>()
                                    .HasKey(j => j.guid)
                                    .HasName("PK_Seeker");

        modelBuilder.Entity<Seeker>()
                                    .HasOne(ps => ps.user)
                                    .WithOne(s => s.seeker)
                                    .HasForeignKey<Seeker>(ps => ps.UserId)
                                    .HasConstraintName("FK_Seeker_UserId");

        //Skills
        modelBuilder.Entity<Skill>()
                                    .HasKey(j => j.guid)
                                    .HasName("PK_Skill");


        //FileModel
        modelBuilder.Entity<FileModel>()
                                    .HasKey(f => f.guid)
                                    .HasName("PK_FileModel");
        modelBuilder.Entity<FileModel>()
                                    .HasOne(f => f.Seeker)
                                    .WithMany(j => j.resumes)
                                    .HasForeignKey(f => f.SeekerId)
                                    .HasConstraintName("FK_FileModel_JobPostId")
                                    .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<FileModel>()
                                    .HasOne(f => f.JobPost)
                                    .WithMany(s => s.resumes)
                                    .HasForeignKey(f => f.JobPostId)
                                    .HasConstraintName("FK_FileModel_SeekerId")
                                    .OnDelete(DeleteBehavior.Restrict);
        //Index
        // Indexes for foreign keys

        // Recruiter
        modelBuilder.Entity<Recruiter>()
            .HasIndex(r => r.UserId)
            .HasDatabaseName("IX_Recruiter_UserId");

        modelBuilder.Entity<Recruiter>()
            .HasIndex(r => r.CompanyId)
            .HasDatabaseName("IX_Recruiter_CompanyId");

        // Company
        modelBuilder.Entity<Company>()
            .HasIndex(c => c.IndustryTypeId)
            .HasDatabaseName("IX_Company_IndustryTypeId");

        // Address
        modelBuilder.Entity<Address>()
            .HasIndex(a => a.companyId)
            .HasDatabaseName("IX_Address_CompanyId");

        // Requirements
        modelBuilder.Entity<Requirements>()
            .HasIndex(r => r.PostId)
            .HasDatabaseName("IX_Requirements_PostId");

        // Responsibilities
        modelBuilder.Entity<Responsibilities>()
            .HasIndex(r => r.PostId)
            .HasDatabaseName("IX_Responsibilities_PostId");

        // PostSkills
        modelBuilder.Entity<PostSkills>()
            .HasIndex(ps => ps.JobPostId)
            .HasDatabaseName("IX_PostSkills_JobPostId");

        modelBuilder.Entity<PostSkills>()
            .HasIndex(ps => ps.SkillId)
            .HasDatabaseName("IX_PostSkills_SkillId");

        // SeekerSkills
        modelBuilder.Entity<SeekerSkills>()
            .HasIndex(ss => ss.SeekerId)
            .HasDatabaseName("IX_SeekerSkills_SeekerId");

        modelBuilder.Entity<SeekerSkills>()
            .HasIndex(ss => ss.SkillId)
            .HasDatabaseName("IX_SeekerSkills_SkillId");

        // JobApplication
        modelBuilder.Entity<JobApplication>()
            .HasIndex(ja => ja.SeekerId)
            .HasDatabaseName("IX_JobApplication_SeekerId");

        modelBuilder.Entity<JobApplication>()
            .HasIndex(ja => ja.JobPostId)
            .HasDatabaseName("IX_JobApplication_JobPostId");

        // JobPost
        modelBuilder.Entity<JobPost>()
            .HasIndex(jp => jp.RecruiterID)
            .HasDatabaseName("IX_JobPost_RecruiterID");

        // Seeker
        modelBuilder.Entity<Seeker>()
            .HasIndex(s => s.UserId)
            .HasDatabaseName("IX_Seeker_UserId");

        // FileModel
        modelBuilder.Entity<FileModel>()
            .HasIndex(f => f.SeekerId)
            .HasDatabaseName("IX_FileModel_SeekerId");

        modelBuilder.Entity<FileModel>()
            .HasIndex(f => f.JobPostId)
            .HasDatabaseName("IX_FileModel_JobPostId");

        // User - Username Index
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique()
            .HasDatabaseName("IX_User_Username");

    }

}

