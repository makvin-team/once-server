using Microsoft.EntityFrameworkCore.Migrations;
using Once.Domain.Entities.Common;

#nullable disable

namespace Once.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Simplify_FraudScenario_MultiLang : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE fraud_scenarios
                    DROP COLUMN IF EXISTS context,
                    DROP COLUMN IF EXISTS decision_options,
                    DROP COLUMN IF EXISTS evidence,
                    DROP COLUMN IF EXISTS explanation,
                    DROP COLUMN IF EXISTS recommendation,
                    DROP COLUMN IF EXISTS red_flag_options,
                    DROP COLUMN IF EXISTS skills,
                    DROP COLUMN IF EXISTS task;
                """);

            migrationBuilder.Sql("""
                DO $$
                BEGIN
                    IF (SELECT data_type FROM information_schema.columns
                        WHERE table_name = 'fraud_scenarios' AND column_name = 'title') = 'character varying' THEN
                        ALTER TABLE fraud_scenarios
                            ALTER COLUMN title       TYPE jsonb USING jsonb_build_object('uz', title,        'ru', title,        'en', title,        'cyrl', title),
                            ALTER COLUMN description TYPE jsonb USING jsonb_build_object('uz', description,  'ru', description,  'en', description,  'cyrl', description),
                            ALTER COLUMN learner_role TYPE jsonb USING jsonb_build_object('uz', learner_role, 'ru', learner_role, 'en', learner_role, 'cyrl', learner_role);
                    END IF;
                END $$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "fraud_scenarios",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(MultiLanguageField),
                oldType: "jsonb");

            migrationBuilder.AlterColumn<string>(
                name: "learner_role",
                table: "fraud_scenarios",
                type: "text",
                nullable: false,
                oldClrType: typeof(MultiLanguageField),
                oldType: "jsonb");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "fraud_scenarios",
                type: "text",
                nullable: false,
                oldClrType: typeof(MultiLanguageField),
                oldType: "jsonb");

            migrationBuilder.AddColumn<string>(
                name: "context",
                table: "fraud_scenarios",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "decision_options",
                table: "fraud_scenarios",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "evidence",
                table: "fraud_scenarios",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "explanation",
                table: "fraud_scenarios",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "recommendation",
                table: "fraud_scenarios",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "red_flag_options",
                table: "fraud_scenarios",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "skills",
                table: "fraud_scenarios",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "task",
                table: "fraud_scenarios",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
