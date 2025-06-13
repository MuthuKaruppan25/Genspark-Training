using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobPortal.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "industryTypes",
                columns: table => new
                {
                    guid = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndustryType", x => x.guid);
                });

            migrationBuilder.CreateTable(
                name: "skills",
                columns: table => new
                {
                    guid = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skill", x => x.guid);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    guid = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "bytea", nullable: false),
                    HashKey = table.Column<byte[]>(type: "bytea", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: false),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.guid);
                });

            migrationBuilder.CreateTable(
                name: "companies",
                columns: table => new
                {
                    guid = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    WebsiteUrl = table.Column<string>(type: "text", nullable: false),
                    IndustryTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.guid);
                    table.ForeignKey(
                        name: "FK_companies_industryTypes_IndustryTypeId",
                        column: x => x.IndustryTypeId,
                        principalTable: "industryTypes",
                        principalColumn: "guid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "seekers",
                columns: table => new
                {
                    guid = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Experience = table.Column<int>(type: "integer", nullable: false),
                    About = table.Column<string>(type: "text", nullable: false),
                    Education = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    ConnectionId = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seeker", x => x.guid);
                    table.ForeignKey(
                        name: "FK_Seeker_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "address",
                columns: table => new
                {
                    guid = table.Column<Guid>(type: "uuid", nullable: false),
                    AddressLine1 = table.Column<string>(type: "text", nullable: false),
                    AddressLine2 = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    State = table.Column<string>(type: "text", nullable: false),
                    PostalCode = table.Column<string>(type: "text", nullable: false),
                    Country = table.Column<string>(type: "text", nullable: false),
                    AddressType = table.Column<string>(type: "text", nullable: false),
                    companyId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.guid);
                    table.ForeignKey(
                        name: "FK_Address_CompanyId",
                        column: x => x.companyId,
                        principalTable: "companies",
                        principalColumn: "guid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "recruiters",
                columns: table => new
                {
                    guid = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    Designation = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recruiter", x => x.guid);
                    table.ForeignKey(
                        name: "FK_Recruiter_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "companies",
                        principalColumn: "guid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Recruiter_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "guid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "seekerSkills",
                columns: table => new
                {
                    guid = table.Column<Guid>(type: "uuid", nullable: false),
                    SkillId = table.Column<Guid>(type: "uuid", nullable: false),
                    SeekerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seekerkill", x => x.guid);
                    table.ForeignKey(
                        name: "FK_SeekerSkills_PostId",
                        column: x => x.SeekerId,
                        principalTable: "seekers",
                        principalColumn: "guid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SeekerSkills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "skills",
                        principalColumn: "guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "jobPosts",
                columns: table => new
                {
                    guid = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    EmploymentType = table.Column<string>(type: "text", nullable: false),
                    EmploymentPosition = table.Column<string>(type: "text", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    SalaryPackage = table.Column<string>(type: "text", nullable: false),
                    RecruiterID = table.Column<Guid>(type: "uuid", nullable: false),
                    PostedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobPost", x => x.guid);
                    table.ForeignKey(
                        name: "FK_JobPost_RecruiterID",
                        column: x => x.RecruiterID,
                        principalTable: "recruiters",
                        principalColumn: "guid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "fileModels",
                columns: table => new
                {
                    guid = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Data = table.Column<byte[]>(type: "bytea", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    JobPostId = table.Column<Guid>(type: "uuid", nullable: true),
                    SeekerId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileModel", x => x.guid);
                    table.ForeignKey(
                        name: "FK_FileModel_JobPostId",
                        column: x => x.SeekerId,
                        principalTable: "seekers",
                        principalColumn: "guid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FileModel_SeekerId",
                        column: x => x.JobPostId,
                        principalTable: "jobPosts",
                        principalColumn: "guid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "jobApplications",
                columns: table => new
                {
                    guid = table.Column<Guid>(type: "uuid", nullable: false),
                    AppliedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    JobPostId = table.Column<Guid>(type: "uuid", nullable: false),
                    SeekerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobApplication", x => x.guid);
                    table.ForeignKey(
                        name: "FK_JobApplications_PostId",
                        column: x => x.JobPostId,
                        principalTable: "jobPosts",
                        principalColumn: "guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobApplications_SeekerId",
                        column: x => x.SeekerId,
                        principalTable: "seekers",
                        principalColumn: "guid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "postSkills",
                columns: table => new
                {
                    guid = table.Column<Guid>(type: "uuid", nullable: false),
                    JobPostId = table.Column<Guid>(type: "uuid", nullable: false),
                    SkillId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostSkill", x => x.guid);
                    table.ForeignKey(
                        name: "FK_PostSkills_PostId",
                        column: x => x.JobPostId,
                        principalTable: "jobPosts",
                        principalColumn: "guid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostSkills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "skills",
                        principalColumn: "guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "requirements",
                columns: table => new
                {
                    guid = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requirements", x => x.guid);
                    table.ForeignKey(
                        name: "FK_Requirements_PostId",
                        column: x => x.PostId,
                        principalTable: "jobPosts",
                        principalColumn: "guid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "responsibilities",
                columns: table => new
                {
                    guid = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Responsibilities", x => x.guid);
                    table.ForeignKey(
                        name: "FK_Requirements_PostId",
                        column: x => x.PostId,
                        principalTable: "jobPosts",
                        principalColumn: "guid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_address_companyId",
                table: "address",
                column: "companyId");

            migrationBuilder.CreateIndex(
                name: "IX_companies_IndustryTypeId",
                table: "companies",
                column: "IndustryTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_fileModels_JobPostId",
                table: "fileModels",
                column: "JobPostId");

            migrationBuilder.CreateIndex(
                name: "IX_fileModels_SeekerId",
                table: "fileModels",
                column: "SeekerId");

            migrationBuilder.CreateIndex(
                name: "IX_jobApplications_JobPostId",
                table: "jobApplications",
                column: "JobPostId");

            migrationBuilder.CreateIndex(
                name: "IX_jobApplications_SeekerId",
                table: "jobApplications",
                column: "SeekerId");

            migrationBuilder.CreateIndex(
                name: "IX_jobPosts_RecruiterID",
                table: "jobPosts",
                column: "RecruiterID");

            migrationBuilder.CreateIndex(
                name: "IX_postSkills_JobPostId",
                table: "postSkills",
                column: "JobPostId");

            migrationBuilder.CreateIndex(
                name: "IX_postSkills_SkillId",
                table: "postSkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_recruiters_CompanyId",
                table: "recruiters",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_recruiters_UserId",
                table: "recruiters",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_requirements_PostId",
                table: "requirements",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_responsibilities_PostId",
                table: "responsibilities",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_seekers_UserId",
                table: "seekers",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_seekerSkills_SeekerId",
                table: "seekerSkills",
                column: "SeekerId");

            migrationBuilder.CreateIndex(
                name: "IX_seekerSkills_SkillId",
                table: "seekerSkills",
                column: "SkillId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "address");

            migrationBuilder.DropTable(
                name: "fileModels");

            migrationBuilder.DropTable(
                name: "jobApplications");

            migrationBuilder.DropTable(
                name: "postSkills");

            migrationBuilder.DropTable(
                name: "requirements");

            migrationBuilder.DropTable(
                name: "responsibilities");

            migrationBuilder.DropTable(
                name: "seekerSkills");

            migrationBuilder.DropTable(
                name: "jobPosts");

            migrationBuilder.DropTable(
                name: "seekers");

            migrationBuilder.DropTable(
                name: "skills");

            migrationBuilder.DropTable(
                name: "recruiters");

            migrationBuilder.DropTable(
                name: "companies");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "industryTypes");
        }
    }
}
