using Microsoft.EntityFrameworkCore.Migrations;
using Once.Domain.Entities.Common;

#nullable disable

namespace Once.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_FraudScenario_PlayUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "context",
                table: "fraud_scenarios");

            migrationBuilder.DropColumn(
                name: "decision_options",
                table: "fraud_scenarios");

            migrationBuilder.DropColumn(
                name: "evidence",
                table: "fraud_scenarios");

            migrationBuilder.DropColumn(
                name: "explanation",
                table: "fraud_scenarios");

            migrationBuilder.DropColumn(
                name: "recommendation",
                table: "fraud_scenarios");

            migrationBuilder.DropColumn(
                name: "red_flag_options",
                table: "fraud_scenarios");

            migrationBuilder.DropColumn(
                name: "skills",
                table: "fraud_scenarios");

            migrationBuilder.DropColumn(
                name: "task",
                table: "fraud_scenarios");

            migrationBuilder.AlterColumn<MultiLanguageField>(
                name: "title",
                table: "fraud_scenarios",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<MultiLanguageField>(
                name: "learner_role",
                table: "fraud_scenarios",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<MultiLanguageField>(
                name: "description",
                table: "fraud_scenarios",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "play_url",
                table: "fraud_scenarios",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "play_url",
                table: "fraud_scenarios");

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
