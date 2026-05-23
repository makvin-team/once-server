using Microsoft.EntityFrameworkCore;
using Once.Domain.Entities;
using Once.Domain.Enums;
using Once.Infrastructure.Persistence;

namespace Once.Infrastructure.Extensions.Seed;

public static class FraudScenarioSeeder
{
    public static async Task SeedFraudScenariosAsync(this AppDbContext dbContext)
    {
        var hasAny = await dbContext.FraudScenarios
            .AsNoTracking()
            .AnyAsync(s => !s.IsDeleted);

        if (hasAny) return;

        dbContext.FraudScenarios.AddRange(BuildScenarios());
        await dbContext.SaveChangesAsync();
    }

    // ── AML / Compliance ─────────────────────────────────────────────────────

    private static FraudScenario AmlSuspiciousTransaction() => new()
    {
        Title            = "Suspicious Transaction Detection",
        Description      = "Analyze a series of structured cash transactions designed to evade regulatory reporting thresholds and determine the appropriate compliance response.",
        FraudType        = FraudSimType.AmlKyc,
        Difficulty       = FraudSimDifficulty.Advanced,
        RiskLevel        = FraudSimRisk.High,
        EstimatedMinutes = 15,
        PassScore        = 75,
        Skills           = """["AML Detection","Transaction Monitoring","Regulatory Reporting","Structuring Identification"]""",
        Context          = "You are reviewing the account of 'Sunrise Trading LLC', a small import/export firm. Over the past 30 days, the account has received 18 cash deposits ranging from $8,400 to $9,800 — all just below the $10,000 CTR threshold. The deposits are followed by immediate wire transfers to three overseas accounts.",
        LearnerRole      = "AML Compliance Officer at National Bank",
        Task             = "Review the transaction evidence and identify all red flags that indicate potential money laundering. Then select the correct course of action.",
        Explanation      = "The pattern of deposits consistently below the $10,000 CTR threshold is a textbook structuring (smurfing) indicator — a federal offense under 31 U.S.C. § 5324. Combined with immediate overseas transfers to high-risk jurisdictions, this requires immediate SAR filing.",
        Recommendation   = "File a SAR with FinCEN within 30 days of detection. Preserve all transaction records. Do not notify the customer (tipping-off prohibition). Refer to your BSA Officer for potential account closure.",
        Evidence         = """[{"id":"e1","label":"Transaction Log (30 days)","detail":"18 cash deposits: $8,400–$9,800. All followed within 24h by wire transfers to UAE, Panama, and Hong Kong."},{"id":"e2","label":"Business Profile","detail":"Sunrise Trading LLC: 2 employees, annual revenue $120K. No import/export licenses on file."},{"id":"e3","label":"Wire Beneficiaries","detail":"Three recurring beneficiaries in UAE and Panama — no invoices or contracts provided for these transfers."}]""",
        RedFlagOptions   = """[{"id":"r1","label":"All deposits are just below the $10,000 CTR filing threshold","correct":true},{"id":"r2","label":"The company has two employees — consistent with its reported revenue","correct":false},{"id":"r3","label":"Deposits are immediately followed by overseas wire transfers with no business documentation","correct":true},{"id":"r4","label":"Wire beneficiaries are in high-risk jurisdictions with no supporting invoices","correct":true},{"id":"r5","label":"The account has been open for more than 12 months","correct":false}]""",
        DecisionOptions  = """[{"id":"d1","label":"File a Suspicious Activity Report (SAR) with FinCEN — do not alert the customer","correct":true},{"id":"d2","label":"Contact the customer and request an explanation before taking action","correct":false},{"id":"d3","label":"Close the account immediately and return funds","correct":false},{"id":"d4","label":"Continue monitoring for 90 more days to build a stronger case","correct":false}]""",
    };

    private static FraudScenario AmlKycBeneficialOwnership() => new()
    {
        Title            = "KYC Mastery – Beneficial Ownership",
        Description      = "Navigate a complex corporate ownership structure to identify the ultimate beneficial owners and assess hidden PEP exposure.",
        FraudType        = FraudSimType.AmlKyc,
        Difficulty       = FraudSimDifficulty.Intermediate,
        RiskLevel        = FraudSimRisk.High,
        EstimatedMinutes = 12,
        PassScore        = 70,
        Skills           = """["KYC","Beneficial Ownership","PEP Screening","Corporate Structure Analysis"]""",
        Context          = "'Alpha Holdings Ltd' is applying for a corporate account with an initial deposit of $2.5 million. The ownership structure shows three corporate shareholders: a BVI shell (42%), a Cyprus entity (35%), and a local holding (23%). The BVI shell is owned by a nominee director with no trace.",
        LearnerRole      = "KYC Analyst at the bank's onboarding team",
        Task             = "Determine whether beneficial ownership requirements are satisfied and identify any red flags that require enhanced due diligence or rejection.",
        Explanation      = "FinCEN's Beneficial Ownership Rule requires identifying all natural persons with 25%+ ownership. The BVI nominee structure obscures the true owner of 42% of the entity. This mandatory EDD trigger cannot be waived. The unknown BVI owner may be a PEP.",
        Recommendation   = "Decline to open the account until the BVI nominee structure is unwound and the natural person beneficial owner of the 42% stake is identified, verified, and screened for PEP/sanctions. Document the decision.",
        Evidence         = """[{"id":"e1","label":"Corporate Registry","detail":"Alpha Holdings Ltd — registered in Cayman Islands. Three shareholders listed as legal entities, no natural persons disclosed."},{"id":"e2","label":"BVI Shell Structure","detail":"42% owned by 'Mercer Nominees Ltd' (BVI). Director: nominee service. Ultimate beneficiary: undisclosed."},{"id":"e3","label":"Initial Deposit Source","detail":"$2.5M described as 'investment capital' — no source of funds documentation provided."}]""",
        RedFlagOptions   = """[{"id":"r1","label":"Ultimate beneficial owner of the 42% BVI stake cannot be identified","correct":true},{"id":"r2","label":"The Cyprus entity holds exactly 35% — below the 25% threshold","correct":false},{"id":"r3","label":"No natural persons are disclosed as beneficial owners despite complex layered structure","correct":true},{"id":"r4","label":"$2.5M initial deposit with no documented source of funds","correct":true},{"id":"r5","label":"The company is incorporated in the Cayman Islands","correct":false}]""",
        DecisionOptions  = """[{"id":"d1","label":"Decline account opening until true beneficial owners are identified and verified","correct":true},{"id":"d2","label":"Open the account with enhanced monitoring — collect ownership docs within 90 days","correct":false},{"id":"d3","label":"Accept — the disclosed ownership already covers 77%, which meets regulatory requirements","correct":false},{"id":"d4","label":"Refer to the relationship manager to negotiate disclosure directly with the client","correct":false}]""",
    };

    private static FraudScenario AmlSanctionsScreening() => new()
    {
        Title            = "Sanctions Screening (OFAC/UN)",
        Description      = "Evaluate a sanctions screening alert on an incoming wire transfer and determine whether it is a true match or false positive.",
        FraudType        = FraudSimType.AmlKyc,
        Difficulty       = FraudSimDifficulty.Advanced,
        RiskLevel        = FraudSimRisk.High,
        EstimatedMinutes = 15,
        PassScore        = 80,
        Skills           = """["OFAC Sanctions","False Positive Analysis","Wire Transfer Compliance","SDN List Navigation"]""",
        Context          = "A $450,000 incoming wire from Dubai is addressed to 'Kalashnikov Trading Co.' Your OFAC screening system generates an 87% name-match alert against an SDN-listed entity 'Kalashnikov International Exports LLC'. The wire purpose states 'agricultural machinery.'",
        LearnerRole      = "Payments Compliance Analyst",
        Task             = "Assess the screening alert. Identify whether sufficient evidence exists to confirm a true match or clear as a false positive. Select the correct action.",
        Explanation      = "An 87% name-match alone does not confirm a true OFAC hit. Analysts must compare all available identifiers (address, country, business type, TIN). In this case the names differ ('Trading Co' vs 'International Exports LLC'), the beneficiary's registration details do not match the SDN entry, and the SDN-listed entity is in Venezuela — not Dubai. This is a false positive, but the review must be documented.",
        Recommendation   = "Clear as a false positive after thorough review. Document your analysis in the case management system including all identifiers compared. Release the wire. Retain records for 5 years per OFAC requirements.",
        Evidence         = """[{"id":"e1","label":"SWIFT Message","detail":"MT103 — $450,000 USD. Beneficiary: Kalashnikov Trading Co, IBAN: AE460330000010925874394, Dubai, UAE. Purpose: agricultural machinery."},{"id":"e2","label":"SDN Entry","detail":"KALASHNIKOV INTERNATIONAL EXPORTS LLC — Venezuela. TIN: VE-88291047. Designated: 2021-03-15. Reason: arms trafficking."},{"id":"e3","label":"Beneficiary Registry","detail":"Kalashnikov Trading Co — registered Dubai, UAE 2018. TIN: AE-77430219. Primary business: agricultural equipment import."}]""",
        RedFlagOptions   = """[{"id":"r1","label":"Name similarity score is 87% — above internal escalation threshold","correct":true},{"id":"r2","label":"Entity name, country, TIN, and business type all differ from the SDN entry","correct":false},{"id":"r3","label":"The beneficiary's country (UAE) does not match the SDN entity's country (Venezuela)","correct":false},{"id":"r4","label":"The wire purpose (agricultural machinery) is consistent with the beneficiary's registered business","correct":false}]""",
        DecisionOptions  = """[{"id":"d1","label":"Clear as false positive — document all identifiers reviewed and release the wire","correct":true},{"id":"d2","label":"Block the wire and report to OFAC as a potential sanctions violation","correct":false},{"id":"d3","label":"Hold the wire indefinitely pending OFAC guidance","correct":false},{"id":"d4","label":"Return funds to sender without investigation","correct":false}]""",
    };

    private static FraudScenario AmlPepRisk() => new()
    {
        Title            = "PEP Risk Assessment",
        Description      = "Identify politically exposed person (PEP) risk in a new account application and apply the correct enhanced due diligence protocol.",
        FraudType        = FraudSimType.AmlKyc,
        Difficulty       = FraudSimDifficulty.Intermediate,
        RiskLevel        = FraudSimRisk.High,
        EstimatedMinutes = 12,
        PassScore        = 70,
        Skills           = """["PEP Screening","Enhanced Due Diligence","Source of Wealth","Risk Classification"]""",
        Context          = "A personal banking application is submitted by Ms. Aigerim Bekova, spouse of the Minister of Natural Resources of Kazakhstan. She is requesting a private banking relationship with an initial deposit of $800,000, described as 'personal savings from jewelry business.'",
        LearnerRole      = "Private Banking Relationship Manager",
        Task             = "Identify PEP exposure indicators and determine the enhanced due diligence actions required before proceeding with the account relationship.",
        Explanation      = "As the spouse of a senior government official, Ms. Bekova is classified as a PEP associate (FATF Recommendation 12). PEP accounts require senior management approval, enhanced source of wealth documentation, and ongoing enhanced monitoring. Her jewelry business must be verified as capable of generating $800K.",
        Recommendation   = "Classify as PEP — initiate Enhanced Due Diligence. Obtain: audited business financials, source of funds documentation, senior management sign-off. Do not open the account until EDD is completed and approved.",
        Evidence         = """[{"id":"e1","label":"Application Form","detail":"Aigerim Bekova. Occupation: Jewelry business owner. Spouse: Minister of Natural Resources, Kazakhstan. Deposit: $800,000 — 'personal savings.'"},{"id":"e2","label":"Business Verification","detail":"Bekova Jewelry Studio — registered 2021. Annual turnover: $45,000 (last filed return). No employees."},{"id":"e3","label":"PEP Screening Result","detail":"Applicant: no direct PEP listing. Spouse: Confirmed PEP — Minister of Natural Resources since 2019."}]""",
        RedFlagOptions   = """[{"id":"r1","label":"Applicant is the spouse of a confirmed senior government official (PEP associate)","correct":true},{"id":"r2","label":"Initial deposit of $800,000 is inconsistent with the jewelry business turnover of $45,000/year","correct":true},{"id":"r3","label":"The jewelry business has been registered for 5 years","correct":false},{"id":"r4","label":"No clear documentation of the source of the $800,000 deposit","correct":true},{"id":"r5","label":"The applicant holds no public office herself","correct":false}]""",
        DecisionOptions  = """[{"id":"d1","label":"Classify as PEP associate — conduct full EDD with senior management approval before opening","correct":true},{"id":"d2","label":"Open account as standard — applicant holds no public office herself","correct":false},{"id":"d3","label":"Decline immediately — bank policy prohibits PEP-associated accounts","correct":false},{"id":"d4","label":"Open account provisionally with enhanced monitoring for 6 months","correct":false}]""",
    };

    private static FraudScenario AmlSarWriting() => new()
    {
        Title            = "SAR Writing",
        Description      = "Construct a complete and legally compliant Suspicious Activity Report for a confirmed money laundering case.",
        FraudType        = FraudSimType.AmlKyc,
        Difficulty       = FraudSimDifficulty.Advanced,
        RiskLevel        = FraudSimRisk.High,
        EstimatedMinutes = 15,
        PassScore        = 75,
        Skills           = """["SAR Filing","Narrative Writing","BSA Compliance","Financial Crime Documentation"]""",
        Context          = "After a 60-day investigation, your team has confirmed that 'GreenPath Construction' has been using the bank to layer illicit funds through inflated contractor invoices. Total suspicious activity: $1.2M across 14 transactions. You must now file a SAR.",
        LearnerRole      = "BSA/AML Officer",
        Task             = "Identify the required elements of a complete SAR and select all items that MUST be included in the report to meet FinCEN requirements.",
        Explanation      = "A complete SAR must include: subject identifying information, account numbers, activity dates and amounts, a clear narrative explaining why the activity is suspicious, and the type of suspicious activity. The narrative must describe the 5 W's (who, what, when, where, why). Tipping off the subject is a federal offense.",
        Recommendation   = "File the SAR within 30 days of initial detection (or 60 days if no subject can be identified). Maintain confidentiality. Preserve all supporting documentation for 5 years. Schedule a 90-day review to determine if continued SAR filing is warranted.",
        Evidence         = """[{"id":"e1","label":"Investigation Summary","detail":"GreenPath Construction — 14 transactions totaling $1.2M identified as layering via inflated invoices to shell contractor 'BuildRight LLC.'"},{"id":"e2","label":"Subject Profile","detail":"GreenPath Construction Inc. — EIN: 47-1923847, Account: ****4421, Owner: David Mercer DOB 1975-03-12."},{"id":"e3","label":"Prior SARs","detail":"No prior SARs filed on this account. First detection flagged by transaction monitoring on Day 1 of investigation."}]""",
        RedFlagOptions   = """[{"id":"r1","label":"Include subject's full legal name, EIN, date of birth, and account number","correct":true},{"id":"r2","label":"Contact the subject to obtain an explanation before filing","correct":false},{"id":"r3","label":"Document all suspicious transactions with dates, amounts, and counterparties","correct":true},{"id":"r4","label":"Write a clear narrative explaining why the activity is suspicious (5 W's)","correct":true},{"id":"r5","label":"Include the name of the bank employee who first detected the activity","correct":false}]""",
        DecisionOptions  = """[{"id":"d1","label":"File complete SAR within 30 days — maintain strict confidentiality and preserve records for 5 years","correct":true},{"id":"d2","label":"Notify law enforcement before filing the SAR to coordinate the investigation","correct":false},{"id":"d3","label":"File a partial SAR now and supplement it once the investigation is fully closed","correct":false},{"id":"d4","label":"Close the account first, then file the SAR to prevent further suspicious activity","correct":false}]""",
    };

    // ── Cybersecurity / InfoSec ──────────────────────────────────────────────

    private static FraudScenario CyberPhishingTriage() => new()
    {
        Title            = "Phishing Triage",
        Description      = "Analyze a suspicious email reported by an employee and determine whether it is a phishing attempt requiring escalation.",
        FraudType        = FraudSimType.Phishing,
        Difficulty       = FraudSimDifficulty.Beginner,
        RiskLevel        = FraudSimRisk.Medium,
        EstimatedMinutes = 8,
        PassScore        = 60,
        Skills           = """["Phishing Detection","Email Header Analysis","Social Engineering Recognition","Incident Triage"]""",
        Context          = "An employee forwarded an email claiming to be from 'IT Support' asking all staff to verify their credentials via a link due to an 'urgent system migration.' The email was sent from it-support@bank-secure-portal.net.",
        LearnerRole      = "Security Analyst (Tier 1 SOC)",
        Task             = "Examine the email evidence and identify all phishing indicators. Select the correct response action.",
        Explanation      = "The sender domain 'bank-secure-portal.net' does not match the bank's domain. The link preview shows a different URL than displayed. The message creates urgency and threatens account suspension — classic social engineering. No legitimate IT department requests credentials by email.",
        Recommendation   = "Mark as confirmed phishing. Block the sender domain in the email gateway. Report to the phishing intelligence feed. Notify all staff not to click. Check if any employees clicked the link and reset credentials if compromised.",
        Evidence         = """[{"id":"e1","label":"Email Headers","detail":"From: it-support@bank-secure-portal.net. Reply-To: harvest@phish99.ru. Received via: phish99.ru mail server."},{"id":"e2","label":"Email Body","detail":"'Urgent: Complete your account verification within 2 hours or your access will be suspended. Click here: [Verify Now]'"},{"id":"e3","label":"Link Preview","detail":"Displayed text: 'https://bank-internal.com/verify'. Actual URL: 'http://bank-secure-portal.net/steal?tok=xyz123'"}]""",
        RedFlagOptions   = """[{"id":"r1","label":"Sender domain (bank-secure-portal.net) does not match the bank's official domain","correct":true},{"id":"r2","label":"Email creates urgency with a 2-hour deadline and threat of account suspension","correct":true},{"id":"r3","label":"Actual link URL differs from the displayed link text","correct":true},{"id":"r4","label":"Reply-To address is a .ru domain — different from the sender domain","correct":true},{"id":"r5","label":"The email is addressed to 'all staff'","correct":false}]""",
        DecisionOptions  = """[{"id":"d1","label":"Confirm phishing — block sender domain, alert all staff, check for click-throughs","correct":true},{"id":"d2","label":"Forward to IT department to verify if they sent it before taking action","correct":false},{"id":"d3","label":"Delete without action — no harm done if no one clicked","correct":false},{"id":"d4","label":"Reply to ask for verification of the sender's identity","correct":false}]""",
    };

    private static FraudScenario CyberSocAlertTriage() => new()
    {
        Title            = "SOC Alert Triage",
        Description      = "Triage a SIEM alert for a successful login from an anomalous location following multiple failed attempts and determine severity and next steps.",
        FraudType        = FraudSimType.Phishing,
        Difficulty       = FraudSimDifficulty.Intermediate,
        RiskLevel        = FraudSimRisk.Medium,
        EstimatedMinutes = 12,
        PassScore        = 65,
        Skills           = """["SIEM Analysis","Threat Classification","Account Compromise Detection","Incident Escalation"]""",
        Context          = "SIEM Alert (HIGH): User 'jsmith' — 12 failed logins from IP 91.185.23.4 (Romania) at 02:14 AM, followed by successful login at 02:21 AM from the same IP. Normal login pattern: weekdays 08:00–18:00 from 192.168.1.x (internal). User has admin access to core banking system.",
        LearnerRole      = "SOC Analyst (Level 2)",
        Task             = "Assess the alert severity, identify compromise indicators, and select the correct immediate response.",
        Explanation      = "This presents a credential compromise pattern: brute-force followed by success, from a foreign IP, at an abnormal time, for a privileged account. This is a P1 security incident. Immediate containment is required — disable the session and reset credentials before investigating how the credentials were obtained.",
        Recommendation   = "Immediately disable the active session and lock the account. Escalate as a P1 incident. Notify the user and IT security management. Forensically preserve SIEM logs. Investigate credential exposure source (phishing, dark web, etc.).",
        Evidence         = """[{"id":"e1","label":"SIEM Alert Details","detail":"User: jsmith | 12 failed attempts 02:14 AM → Successful login 02:21 AM | Source IP: 91.185.23.4 (Bucharest, RO) | Threat Intel: IP flagged in 3 breach databases."},{"id":"e2","label":"User Profile","detail":"John Smith — Senior Systems Administrator. Access: Core banking system (admin), customer data warehouse, treasury operations. Last known login: yesterday 17:42 from 192.168.1.54."},{"id":"e3","label":"GeoIP Data","detail":"Normal login location: headquarter office (IP range 192.168.1.0/24). Current session origin: Bucharest, Romania — 8,900 km from office."}]""",
        RedFlagOptions   = """[{"id":"r1","label":"Successful login followed 12 failed attempts — consistent with brute-force attack","correct":true},{"id":"r2","label":"Login originated from a foreign country, 8,900 km from the user's normal location","correct":true},{"id":"r3","label":"Login occurred at 02:21 AM — outside the user's normal working hours","correct":true},{"id":"r4","label":"The user has administrative access to core banking systems","correct":true},{"id":"r5","label":"The user's account has been active for more than 3 years","correct":false}]""",
        DecisionOptions  = """[{"id":"d1","label":"Immediately disable active session and lock account — escalate as P1 incident","correct":true},{"id":"d2","label":"Monitor the session in real-time before deciding — may be a VPN connection","correct":false},{"id":"d3","label":"Send an automated alert to the user via email and await their response","correct":false},{"id":"d4","label":"Mark as low priority — foreign logins are common for traveling staff","correct":false}]""",
    };

    private static FraudScenario CyberIncidentResponse() => new()
    {
        Title            = "Incident Response (NIST)",
        Description      = "Lead the response to a ransomware attack on the bank's network using the NIST Incident Response Framework.",
        FraudType        = FraudSimType.Phishing,
        Difficulty       = FraudSimDifficulty.Advanced,
        RiskLevel        = FraudSimRisk.High,
        EstimatedMinutes = 15,
        PassScore        = 75,
        Skills           = """["NIST IR Framework","Ransomware Response","Containment Strategy","Business Continuity"]""",
        Context          = "09:15 AM: Multiple file servers display ransom notes. Ransomware 'LockBit 3.0' has encrypted 60% of file servers. Three domain admin accounts are compromised. Data exfiltration signs detected. Backup systems are on the same network segment.",
        LearnerRole      = "Incident Response Lead",
        Task             = "Identify the correct NIST-aligned response priorities and actions for the first hour of this incident.",
        Explanation      = "Per NIST SP 800-61r2, the correct sequence is: Detect → Contain → Eradicate → Recover. Immediate priorities: isolate affected systems (containment), preserve evidence, activate incident response plan, notify executive management and legal. Do NOT pay the ransom — it does not guarantee decryption and funds criminal activity.",
        Recommendation   = "Immediately isolate affected network segments. Preserve forensic evidence before any remediation. Activate Business Continuity Plan. Engage incident response retainer. Notify board, legal, and regulators per breach notification requirements. Begin recovery from air-gapped backups.",
        Evidence         = """[{"id":"e1","label":"Ransom Note","detail":"'Your files are encrypted by LockBit 3.0. Pay 50 BTC within 72 hours or data will be published. Contact: lockbit3-support.onion'"},{"id":"e2","label":"Affected Systems","detail":"File servers: 18/30 encrypted. Domain controllers: 2 compromised. Backup server: same VLAN — potentially accessible to attacker."},{"id":"e3","label":"Data Exfiltration Indicators","detail":"Unusual outbound traffic detected: 340 GB transferred to external IP 185.220.101.x (known Tor exit node) between 03:00–08:45 AM."}]""",
        RedFlagOptions   = """[{"id":"r1","label":"Backup servers are on the same network segment — potentially compromised","correct":true},{"id":"r2","label":"Domain admin credentials have been compromised — attacker has elevated access","correct":true},{"id":"r3","label":"340 GB exfiltrated before ransomware deployment — double extortion risk","correct":true},{"id":"r4","label":"Ransomware has only affected 60% of servers — unaffected servers can continue operating normally","correct":false},{"id":"r5","label":"The 72-hour ransom deadline creates pressure to pay quickly","correct":false}]""",
        DecisionOptions  = """[{"id":"d1","label":"Isolate all affected segments immediately — activate IR plan, preserve evidence, recover from air-gapped backups","correct":true},{"id":"d2","label":"Pay the ransom to restore systems quickly and minimize business disruption","correct":false},{"id":"d3","label":"Restore from backup immediately before containing the spread","correct":false},{"id":"d4","label":"Continue operating unaffected systems normally while investigating the encryption cause","correct":false}]""",
    };

    private static FraudScenario CyberZeroTrustAccess() => new()
    {
        Title            = "Zero Trust Access",
        Description      = "Evaluate an emergency access request against Zero Trust principles and determine whether to grant access from an unmanaged device.",
        FraudType        = FraudSimType.Phishing,
        Difficulty       = FraudSimDifficulty.Intermediate,
        RiskLevel        = FraudSimRisk.Medium,
        EstimatedMinutes = 10,
        PassScore        = 65,
        Skills           = """["Zero Trust Architecture","Device Compliance","Access Policy","Identity Verification"]""",
        Context          = "A developer submits an emergency access ticket at 11 PM requesting production database credentials. They claim their work laptop was stolen and they are using a personal laptop. The access is needed to 'fix a critical production bug.' The device fails MDM enrollment and has no endpoint protection.",
        LearnerRole      = "IT Security Manager",
        Task             = "Apply Zero Trust principles to this access request and determine the correct response.",
        Explanation      = "Zero Trust mandates 'never trust, always verify' — every access request must meet all policy controls regardless of claimed emergency. An unmanaged, non-compliant device cannot access production systems. The correct path is to provide a managed loaner device or establish a secure jump host session. Emergencies do not override Zero Trust controls.",
        Recommendation   = "Deny direct access from the unmanaged device. Offer alternatives: issue a managed loaner, establish a RDP session from a compliant bastion host, or coordinate with on-call colleague using a managed device. Document the access request and decision.",
        Evidence         = """[{"id":"e1","label":"Access Request","detail":"Developer: Alex Kim | Request: Production DB credentials | Device: Personal MacBook (unmanaged) | MDM Status: Not enrolled | Endpoint Protection: None | Location: Home (verified)"},{"id":"e2","label":"Device Compliance Report","detail":"Device fingerprint not in approved device inventory. OS: macOS 13.2 (unpatched — 3 critical CVEs). No encryption verification possible. Fails all compliance checks."},{"id":"e3","label":"Incident Ticket","detail":"Production bug reported 10:45 PM. Severity: P2 (affecting 2% of transactions). On-call engineer: Sarah Lee (managed device, available)."}]""",
        RedFlagOptions   = """[{"id":"r1","label":"Device is unmanaged, not in approved inventory, and fails all compliance checks","correct":true},{"id":"r2","label":"Device OS has 3 unpatched critical CVEs","correct":true},{"id":"r3","label":"Emergency request bypasses normal approval workflow","correct":true},{"id":"r4","label":"The production bug severity is P2 — a qualified on-call engineer is available","correct":true},{"id":"r5","label":"The developer's identity has been verified via corporate SSO","correct":false}]""",
        DecisionOptions  = """[{"id":"d1","label":"Deny — direct access from non-compliant device violates Zero Trust. Redirect to on-call engineer or managed jump host","correct":true},{"id":"d2","label":"Grant temporary 2-hour credentials — emergency justifies exception","correct":false},{"id":"d3","label":"Grant access with enhanced logging — the risk is acceptable given the emergency","correct":false},{"id":"d4","label":"Escalate to CISO to authorize the exception","correct":false}]""",
    };

    private static FraudScenario CyberDeepfakeVoice() => new()
    {
        Title            = "Deepfake Voice Verification",
        Description      = "Detect deepfake voice fraud in a high-value wire transfer call and apply the correct multi-factor verification protocol.",
        FraudType        = FraudSimType.DeepfakeCall,
        Difficulty       = FraudSimDifficulty.Advanced,
        RiskLevel        = FraudSimRisk.High,
        EstimatedMinutes = 12,
        PassScore        = 75,
        Skills           = """["Deepfake Detection","Voice Authentication","Social Engineering Defense","Wire Fraud Prevention"]""",
        Context          = "A caller claims to be James Hartley (CEO of Hartley Capital, a $5M account holder) requesting an urgent $380,000 wire to a new beneficiary in Singapore for a 'time-sensitive acquisition.' The voice sounds authentic. The callback number differs from the registered mobile. The CEO's assistant is 'unreachable.'",
        LearnerRole      = "Customer Service Security Specialist",
        Task             = "Identify deepfake and social engineering indicators, then choose the correct verification and response protocol.",
        Explanation      = "Deepfake voice fraud involves AI-synthesized audio mimicking a known person's voice. Key tells: unexpected wire request, new beneficiary, urgency narrative, unavailable verification contacts, callback number mismatch. Protocol: never execute on voice alone — require multi-factor out-of-band verification via registered contact details.",
        Recommendation   = "Do not process the wire. Contact the CEO via the registered phone number (not the caller-provided number). Require written authorization via secure email. Engage fraud team immediately. Brief the real James Hartley if confirmed as impersonation.",
        Evidence         = """[{"id":"e1","label":"Call Details","detail":"Inbound call — caller ID: +1-917-555-0134. Registered mobile on file: +1-212-555-0189. Caller voice: sounds like James Hartley. Request: $380,000 wire to DBS Bank Singapore, Beneficiary: 'Meridian Acquisitions Pte Ltd' (not on approved beneficiary list)."},{"id":"e2","label":"Audio Analysis (AI)","detail":"Real-time voice analysis: 73% probability of AI synthesis. Micro-pause patterns inconsistent with natural speech. Emotional prosody anomalies detected."},{"id":"e3","label":"Verification Attempts","detail":"CEO personal assistant: voicemail. CEO registered email: no response in 8 minutes. New beneficiary 'Meridian Acquisitions': registered 11 days ago, no web presence."}]""",
        RedFlagOptions   = """[{"id":"r1","label":"Callback number does not match the registered mobile number on file","correct":true},{"id":"r2","label":"AI voice analysis shows 73% probability of synthetic audio","correct":true},{"id":"r3","label":"Requested beneficiary is new, unverified, and registered only 11 days ago","correct":true},{"id":"r4","label":"All verification contacts are unavailable — isolation tactic","correct":true},{"id":"r5","label":"The wire amount ($380K) is within the CEO's typical transaction range","correct":false}]""",
        DecisionOptions  = """[{"id":"d1","label":"Refuse to process — contact CEO via registered number, require written authorization via secure email","correct":true},{"id":"d2","label":"Process the wire — the voice is authentic and the amount is within normal limits","correct":false},{"id":"d3","label":"Ask the caller three security questions and process if answers are correct","correct":false},{"id":"d4","label":"Request the caller to call back from the registered number to proceed","correct":false}]""",
    };

    // ── Fraud Monitoring ─────────────────────────────────────────────────────

    private static FraudScenario FraudVelocityPattern() => new()
    {
        Title            = "Velocity Pattern Detection",
        Description      = "Analyze a high-velocity debit card transaction pattern on a pensioner's account to determine if it represents fraud.",
        FraudType        = FraudSimType.Transaction,
        Difficulty       = FraudSimDifficulty.Intermediate,
        RiskLevel        = FraudSimRisk.High,
        EstimatedMinutes = 12,
        PassScore        = 70,
        Skills           = """["Transaction Velocity Analysis","Behavioral Analytics","Fraud Pattern Recognition","Card Fraud Response"]""",
        Context          = "Fraud alert: Debit card *4482 (Margaret Wilson, 67, pensioner) — 15 transactions at 8 merchants in 45 minutes. Merchants: 3 electronics stores, 2 jewelry stores, gaming store, department store, and online retailer. Total: $4,200. Normal pattern: weekly grocery and pharmacy purchases under $150.",
        LearnerRole      = "Fraud Analyst",
        Task             = "Analyze the transaction velocity and behavioral pattern. Determine whether this represents fraud and select the appropriate response.",
        Explanation      = "This pattern — 15 transactions in 45 minutes across geographically dispersed merchants, with amounts and merchant categories completely inconsistent with the customer's 3-year behavioral profile — strongly indicates card-not-present fraud after card data compromise. Pensioner age profile amplifies the risk.",
        Recommendation   = "Block the card immediately. Attempt to contact the customer via registered phone number. Issue fraud case and initiate dispute process for unauthorized transactions. Identify the compromise point.",
        Evidence         = """[{"id":"e1","label":"Transaction Log","detail":"15 transactions: 09:03–09:48 AM. Electronics: $890, $1,240, $740. Jewelry: $620, $510. Gaming: $190. Online retailer: $10 (authorization test). All card-not-present except 2."},{"id":"e2","label":"Behavioral Baseline (12 months)","detail":"Average monthly spend: $340. Merchant categories: grocery (67%), pharmacy (22%), occasional restaurant. Maximum single transaction: $145. Never: electronics, jewelry, gaming."},{"id":"e3","label":"Cardholder Profile","detail":"Margaret Wilson — 67 years old. Retired teacher. Monthly income: $1,840 (pension). Account open 11 years. No prior fraud history. Card issued 8 months ago."}]""",
        RedFlagOptions   = """[{"id":"r1","label":"15 transactions in 45 minutes — extreme velocity vs. customer's normal 3-4 weekly transactions","correct":true},{"id":"r2","label":"Merchant categories (electronics, jewelry, gaming) are completely inconsistent with 12-month behavioral profile","correct":true},{"id":"r3","label":"Small $10 authorization at online retailer — typical 'card testing' pattern","correct":true},{"id":"r4","label":"Transaction velocity is high but the total amount ($4,200) is within monthly card limit","correct":false},{"id":"r5","label":"All merchant transactions are in the same city as the cardholder","correct":false}]""",
        DecisionOptions  = """[{"id":"d1","label":"Block card immediately and contact customer — initiate fraud investigation","correct":true},{"id":"d2","label":"Flag for 24-hour monitoring — allow transactions to continue pending review","correct":false},{"id":"d3","label":"Send an SMS alert to the customer and wait for their response before blocking","correct":false},{"id":"d4","label":"Increase the card's fraud score threshold — current pattern may be vacation spending","correct":false}]""",
    };

    private static FraudScenario FraudMuleAccount() => new()
    {
        Title            = "Mule Account Spotting",
        Description      = "Identify money mule account indicators from transaction patterns and customer behavior, and take appropriate action.",
        FraudType        = FraudSimType.Transaction,
        Difficulty       = FraudSimDifficulty.Advanced,
        RiskLevel        = FraudSimRisk.High,
        EstimatedMinutes = 15,
        PassScore        = 75,
        Skills           = """["Money Mule Detection","Network Analysis","Pass-Through Pattern Recognition","Financial Investigation"]""",
        Context          = "Account holder: Kevin Ochoa (23, student, no employment record). Account receives transfers from 12 different individuals (avg. $800 each) three times weekly. Within 24 hours, nearly identical amounts are wired to two accounts in Nigeria and Georgia. Balance rarely exceeds $200.",
        LearnerRole      = "Fraud Investigator",
        Task             = "Identify money mule indicators and determine the correct investigative and reporting actions.",
        Explanation      = "This is a classic money mule pattern: multiple payers → aggregation → immediate overseas forwarding with minimal retention. The student profile, lack of employment income, and 12 different payer network are hallmarks of a recruited mule. This constitutes money laundering and may also indicate the account holder is a victim of recruitment fraud.",
        Recommendation   = "Freeze the account to prevent further money movement. File a SAR immediately. Investigate the account holder to determine if they are a willing participant or a fraud victim. Do not close the account before law enforcement consultation.",
        Evidence         = """[{"id":"e1","label":"Inbound Transfer Pattern","detail":"30-day period: 38 inbound transfers from 12 different individuals. Average: $820. Sources: Zelle, Venmo, bank wire. No consistent payer — new individuals each week."},{"id":"e2","label":"Outbound Transfer Pattern","detail":"37 outbound wires within 24 hours of inbound receipt. Beneficiaries: 'Emeka Finance Ltd' (Lagos, NG) and 'GeoTrans LLC' (Tbilisi, GE). Amounts match inbound net of $5–20 fee retained."},{"id":"e3","label":"Account Holder Profile","detail":"Kevin Ochoa — 23 years old. Student. Occupation: listed as 'freelance consultant.' Verified employment: none. Account opened 4 months ago. Two prior fraud inquiries from sending banks."}]""",
        RedFlagOptions   = """[{"id":"r1","label":"Account aggregates funds from 12 unrelated individuals and immediately forwards overseas","correct":true},{"id":"r2","label":"Account balance rarely exceeds $200 — no legitimate retention of funds","correct":true},{"id":"r3","label":"Beneficiaries in high-risk jurisdictions with no documented business relationship","correct":true},{"id":"r4","label":"Two prior fraud inquiries from other banks about this account","correct":true},{"id":"r5","label":"Account holder is a student — consistent with irregular income from freelancing","correct":false}]""",
        DecisionOptions  = """[{"id":"d1","label":"Freeze account and file SAR — investigate whether account holder is participant or victim","correct":true},{"id":"d2","label":"Close the account immediately and return all funds to senders","correct":false},{"id":"d3","label":"Monitor for 30 more days to confirm the pattern before taking action","correct":false},{"id":"d4","label":"Contact account holder for explanation before restricting access","correct":false}]""",
    };

    private static FraudScenario FraudCardSkimming() => new()
    {
        Title            = "Card Skimming Behavioral",
        Description      = "Investigate an ATM card skimming incident, identify compromised cards, and prioritize actions to minimize fraud losses.",
        FraudType        = FraudSimType.Transaction,
        Difficulty       = FraudSimDifficulty.Intermediate,
        RiskLevel        = FraudSimRisk.Medium,
        EstimatedMinutes = 10,
        PassScore        = 65,
        Skills           = """["Skimming Investigation","Common Point of Purchase Analysis","Card Risk Management","Loss Mitigation"]""",
        Context          = "Five customers report unauthorized transactions in one day. Fraud analysts identify a common ATM used by all five victims in the past 10 days. ATM inspection confirms a skimming device was attached from Day 1–8. 340 cards used the ATM during this period.",
        LearnerRole      = "Fraud Operations Specialist",
        Task             = "Identify the appropriate response to the skimming incident and prioritize card management actions.",
        Explanation      = "When a compromise point is confirmed, all cards used during the compromise window are at risk. The correct approach is to block and reissue all 340 at-risk cards proactively — not just the 5 confirmed fraud cases. Waiting for additional fraud reports increases losses. Law enforcement should be notified in parallel.",
        Recommendation   = "Block all 340 cards that used the compromised ATM between Days 1–8. Proactively reissue cards. Notify affected customers. File law enforcement report. Remove and forensically examine the skimming device. Notify card network (Visa/Mastercard) as required.",
        Evidence         = """[{"id":"e1","label":"ATM Inspection Report","detail":"Skimming device + pinhole camera found on ATM ID 4471. Installation period confirmed: Day 1–8 (removed on inspection Day 10). Card data capture estimated: magnetic stripe + PIN."},{"id":"e2","label":"Common Point of Purchase Analysis","detail":"All 5 fraud victims used ATM 4471 during Days 1–8. Total unique cards used during compromise window: 340. Fraud detected on cards: 5 confirmed (1.5%) — typical early detection rate."},{"id":"e3","label":"Fraud Loss Summary","detail":"Confirmed losses to date: $8,400 across 5 accounts. Projected total exposure (340 cards × avg. fraud pattern): $180,000 if all compromised cards are used."}]""",
        RedFlagOptions   = """[{"id":"r1","label":"Compromise window confirmed (Day 1–8) — 340 cards are at risk, not just the 5 fraud cases","correct":true},{"id":"r2","label":"Only 1.5% of at-risk cards have reported fraud — typical early-detection rate for skimming","correct":true},{"id":"r3","label":"ATM captured magnetic stripe AND PIN — both factors needed for full card cloning are compromised","correct":true},{"id":"r4","label":"Projected exposure ($180K) far exceeds cost of proactive reissuance","correct":true},{"id":"r5","label":"Only cards with confirmed fraud transactions should be blocked to avoid customer disruption","correct":false}]""",
        DecisionOptions  = """[{"id":"d1","label":"Block and proactively reissue all 340 at-risk cards — notify customers and law enforcement","correct":true},{"id":"d2","label":"Block only the 5 confirmed fraud cards — reissue others only as fraud is reported","correct":false},{"id":"d3","label":"Alert law enforcement only — allow customers to decide whether to get new cards","correct":false},{"id":"d4","label":"Monitor all 340 accounts for fraud before blocking — avoid customer disruption","correct":false}]""",
    };

    private static FraudScenario FraudChargebackTriage() => new()
    {
        Title            = "Chargeback Triage",
        Description      = "Evaluate a dispute claim against authentication evidence to determine whether to accept or deny a chargeback.",
        FraudType        = FraudSimType.Transaction,
        Difficulty       = FraudSimDifficulty.Beginner,
        RiskLevel        = FraudSimRisk.Medium,
        EstimatedMinutes = 8,
        PassScore        = 60,
        Skills           = """["Chargeback Investigation","EMV Authentication","Dispute Resolution","Fraud vs. Friendly Fraud"]""",
        Context          = "Customer Lisa Chen disputes a $2,400 purchase at ElectroMax store, claiming 'I did not authorize this.' Transaction record shows chip-and-PIN authentication with the customer's physical card. The customer has filed 3 chargebacks in the past 18 months.",
        LearnerRole      = "Dispute Resolution Specialist",
        Task             = "Evaluate the chargeback claim against the authentication evidence and determine the correct dispute resolution.",
        Explanation      = "EMV chip-and-PIN is the strongest card-present authentication method. Visa CRP §4.7 and EMV liability shift place the liability on the card issuer only if the transaction was card-not-present or used a fallback to magnetic stripe. A successful chip-and-PIN transaction with the physical card strongly refutes a 'did not authorize' claim. Combined with the customer's chargeback history, this is likely friendly fraud.",
        Recommendation   = "Deny the chargeback — the chip-and-PIN authentication provides strong evidence of cardholder presence. Present the EMV authentication log to the card network. Document the customer's chargeback history for pattern analysis.",
        Evidence         = """[{"id":"e1","label":"Transaction Receipt","detail":"ElectroMax — $2,400 | Card: chip insert (no fallback) | Authentication: PIN verified | Terminal: certified EMV POS | Date/Time: Sat 14:32 | Merchant category: electronics."},{"id":"e2","label":"Authentication Log","detail":"Card serial: matches physical card issued to customer | Chip transaction: cryptogram validated by card network | PIN entry: 3 digits entered correctly on first attempt | No velocity flag at time of transaction."},{"id":"e3","label":"Customer Dispute History","detail":"Lisa Chen — 3 chargebacks in 18 months: 2 resolved (1 in customer favor, 1 denied), 1 pending. All disputes: 'did not authorize.' Annual card spend: $18,000."}]""",
        RedFlagOptions   = """[{"id":"r1","label":"Transaction authenticated via chip-and-PIN — physical card and PIN required","correct":false},{"id":"r2","label":"Customer has filed 3 chargebacks in 18 months — above normal frequency","correct":true},{"id":"r3","label":"All previous disputes used the same 'did not authorize' claim","correct":true},{"id":"r4","label":"EMV liability shift does not apply — this was a chip transaction, not magnetic stripe fallback","correct":true},{"id":"r5","label":"Transaction amount ($2,400) is inconsistent with customer's spending profile","correct":false}]""",
        DecisionOptions  = """[{"id":"d1","label":"Deny chargeback — chip-and-PIN authentication confirms cardholder presence. Present EMV log to card network","correct":true},{"id":"d2","label":"Accept chargeback — customer claim of non-authorization must be honored","correct":false},{"id":"d3","label":"Request additional documentation from the merchant before deciding","correct":false},{"id":"d4","label":"Issue provisional credit and continue investigation","correct":false}]""",
    };

    private static FraudScenario FraudAiAnomaly() => new()
    {
        Title            = "AI Anomaly Tuning",
        Description      = "Diagnose a machine learning fraud model's high false positive rate and recommend targeted model improvements.",
        FraudType        = FraudSimType.Transaction,
        Difficulty       = FraudSimDifficulty.Advanced,
        RiskLevel        = FraudSimRisk.High,
        EstimatedMinutes = 15,
        PassScore        = 75,
        Skills           = """["ML Model Evaluation","Feature Engineering","False Positive Reduction","Fraud Model Governance"]""",
        Context          = "Your card fraud ML model is blocking 40% of international transactions as suspected fraud. Customer complaints increased 300% this month. A manual review sample shows: 85% of blocked international transactions are legitimate. Meanwhile, 12 confirmed fraud cases slipped through undetected this week.",
        LearnerRole      = "Fraud Data Scientist",
        Task             = "Identify the root causes of the model's degraded performance and select the correct remediation approach.",
        Explanation      = "A 40% false positive rate with 85% of blocks being legitimate indicates model drift — the model was trained on pre-pandemic data where international travel was less common. The missing fraud cases suggest the model is also missing new attack vectors. The solution is retraining with recent data and adding behavioral features, not disabling international alerts.",
        Recommendation   = "Retrain the model on the past 12 months of data with travel/geolocation behavioral features added. Implement a customer travel notification feature as a positive signal. Add new fraud patterns from the 12 missed cases. Deploy via A/B testing. Do not disable international alerts — increase manual review capacity temporarily.",
        Evidence         = """[{"id":"e1","label":"Model Performance Metrics","detail":"False Positive Rate: 40% (international) vs 3.2% (domestic). False Negative Rate: 1.8% (above acceptable 0.5% threshold). Model last retrained: 18 months ago (pre-reopening travel surge)."},{"id":"e2","label":"Feature Importance Report","detail":"Top triggering features: (1) foreign country = 0.68 weight, (2) transaction > $500 = 0.54 weight, (3) new merchant = 0.41 weight. Missing features: travel mode, customer travel history, behavioral velocity."},{"id":"e3","label":"Missed Fraud Sample","detail":"12 missed fraud cases: all used domestic card data with foreign IP. Pattern: account takeover + VPN usage to appear domestic. This attack vector not present in training data."}]""",
        RedFlagOptions   = """[{"id":"r1","label":"Model was last retrained 18 months ago — before significant changes in international travel patterns","correct":true},{"id":"r2","label":"'Foreign country' has 0.68 feature weight — likely over-indexed causing mass false positives","correct":true},{"id":"r3","label":"Missed fraud uses domestic card data with foreign IP — attack vector absent from training data","correct":true},{"id":"r4","label":"Behavioral features like travel history and velocity are missing from the model","correct":true},{"id":"r5","label":"Increasing the manual review team will permanently solve the model performance problem","correct":false}]""",
        DecisionOptions  = """[{"id":"d1","label":"Retrain with 12-month data, add behavioral features, A/B test before full deployment","correct":true},{"id":"d2","label":"Disable international fraud alerts temporarily to stop customer complaints","correct":false},{"id":"d3","label":"Increase the alert threshold for international transactions until retraining is complete","correct":false},{"id":"d4","label":"Replace ML model with a purely rule-based system for international transactions","correct":false}]""",
    };

    // ── Customer Support / Front Office ──────────────────────────────────────

    private static FraudScenario CxDifficultCustomer() => new()
    {
        Title            = "Difficult-Customer De-escalation",
        Description      = "De-escalate an angry customer threatening regulatory action after an erroneous fee charge while preserving the banking relationship.",
        FraudType        = FraudSimType.SocialEngineering,
        Difficulty       = FraudSimDifficulty.Beginner,
        RiskLevel        = FraudSimRisk.Low,
        EstimatedMinutes = 8,
        PassScore        = 60,
        Skills           = """["De-escalation Techniques","Empathetic Communication","Service Recovery","Complaint Resolution"]""",
        Context          = "Robert Finch (premium customer, 15-year relationship, $250K balance) is furious. He was charged a $35 maintenance fee despite qualifying for fee waiver. He demands to speak to the CEO, threatens to close all accounts and file a regulator complaint. The fee was confirmed as an error.",
        LearnerRole      = "Customer Service Representative",
        Task             = "Identify the correct de-escalation approach and service recovery actions to resolve the situation.",
        Explanation      = "When a fee is confirmed as an error, immediate acknowledgment and reversal is both correct and required. Empathy first — validate the customer's frustration before explaining. A premium 15-year customer threatening to leave represents significant lifetime value at risk. Defensive responses or policy-citing before empathy will escalate the situation.",
        Recommendation   = "Lead with a genuine apology acknowledging the error. Confirm the fee reversal immediately while on the call. Offer a goodwill gesture (e.g., additional service or brief call from branch manager). Document the complaint and fee error for system correction.",
        Evidence         = """[{"id":"e1","label":"Account Details","detail":"Robert Finch — Premium Banking client. Relationship: 15 years. Combined balances: $252,000. Fee waiver qualification: YES (balance > $25,000). Fee charged: $35 maintenance fee — confirmed error in waiver eligibility system."},{"id":"e2","label":"Customer Statement","detail":"'This is completely unacceptable. I have been with this bank for 15 years. I am going to close every account and call the banking regulator about your illegal fees. Get me your CEO right now.'"},{"id":"e3","label":"Prior Complaint History","detail":"Robert Finch — 0 complaints in 15 years. Monthly direct deposit: $12,000. Autopay: mortgage, insurance, utilities through this bank."}]""",
        RedFlagOptions   = """[{"id":"r1","label":"The fee was confirmed as an error — immediate reversal is both correct and required","correct":true},{"id":"r2","label":"Customer has a 15-year zero-complaint history — current anger is proportionate to frustration","correct":true},{"id":"r3","label":"Regulatory complaint threat indicates the customer is aware of their rights","correct":false},{"id":"r4","label":"Defending the fee with policy before acknowledging the error will escalate the situation","correct":true},{"id":"r5","label":"Premium customer status means the bank must escalate to a manager immediately","correct":false}]""",
        DecisionOptions  = """[{"id":"d1","label":"Apologize sincerely, confirm fee reversal immediately on the call, offer goodwill gesture","correct":true},{"id":"d2","label":"Transfer immediately to supervisor — the customer requested CEO contact","correct":false},{"id":"d3","label":"Explain bank policy and fee structure before processing any reversal","correct":false},{"id":"d4","label":"Reverse the fee but note that the customer's tone is unacceptable","correct":false}]""",
    };

    private static FraudScenario CxAddressChange() => new()
    {
        Title            = "Address-Change Verification",
        Description      = "Apply social engineering detection and verification protocol during a caller's account address change request.",
        FraudType        = FraudSimType.SocialEngineering,
        Difficulty       = FraudSimDifficulty.Intermediate,
        RiskLevel        = FraudSimRisk.Medium,
        EstimatedMinutes = 10,
        PassScore        = 65,
        Skills           = """["Social Engineering Detection","Identity Verification","Account Change Security","Call Center Protocol"]""",
        Context          = "A caller provides the account number and date of birth correctly but fails the security question ('mother's maiden name'). They claim to have 'recently moved and forgotten some old details.' They are requesting an address change, and mention they also need to order a new card to the new address.",
        LearnerRole      = "Call Center Security Agent",
        Task             = "Identify social engineering indicators and apply the correct verification and action protocol.",
        Explanation      = "An address change followed immediately by a card request is a classic account takeover (ATO) precursor — redirect card to attacker's address. Partial verification failure must be treated seriously. Correct protocol: deny the change during this call and require verification via a secure alternative channel (in-branch with ID, or mail confirmation to current address).",
        Recommendation   = "Do not process the address change via phone with failed security verification. Send a confirmation letter to the current address on file. Require the customer to verify identity in-branch with government ID. Flag the account for ATO monitoring for 30 days. Document the call.",
        Evidence         = """[{"id":"e1","label":"Verification Scorecard","detail":"Account number: PASS | Date of birth: PASS | Mother's maiden name: FAIL | Last transaction amount: NOT ATTEMPTED. Policy: 3-factor verification required for address change — caller achieved 2/3."},{"id":"e2","label":"Call Context","detail":"Caller claims: 'I just moved last week, I can never remember my mother's maiden name anyway. Just change my address and send my new card there — I need it urgently.' Requested new address: 847 Elm Street, Brooklyn."},{"id":"e3","label":"Account History","detail":"Last 30 days: 3 unsuccessful online login attempts (7 days ago). Address on file unchanged for 4 years. No previous address changes via phone. Prior card: delivered to current address 8 months ago with no issues."}]""",
        RedFlagOptions   = """[{"id":"r1","label":"Failed security question — only 2 of 3 required verification factors passed","correct":true},{"id":"r2","label":"Address change followed immediately by new card request — classic ATO precursor pattern","correct":true},{"id":"r3","label":"3 failed online login attempts 7 days ago on the same account","correct":true},{"id":"r4","label":"Caller creates urgency ('I need it urgently') — social engineering pressure tactic","correct":true},{"id":"r5","label":"Caller correctly provided account number and date of birth","correct":false}]""",
        DecisionOptions  = """[{"id":"d1","label":"Deny address change — send confirmation to current address, require in-branch ID verification","correct":true},{"id":"d2","label":"Process the change — caller passed 2 out of 3 factors, which is sufficient","correct":false},{"id":"d3","label":"Process the change but delay the card order by 5 business days as a precaution","correct":false},{"id":"d4","label":"Transfer to a senior agent who can make an exception for the failed verification","correct":false}]""",
    };

    private static FraudScenario CxAccountBlock() => new()
    {
        Title            = "Account Block Empathy",
        Description      = "Handle an emotionally distressed customer whose account is blocked due to a fraud suspicion, balancing empathy with security protocol.",
        FraudType        = FraudSimType.SocialEngineering,
        Difficulty       = FraudSimDifficulty.Intermediate,
        RiskLevel        = FraudSimRisk.Medium,
        EstimatedMinutes = 10,
        PassScore        = 65,
        Skills           = """["Empathetic Service","Security Protocol Adherence","Emotional Intelligence","Fraud Block Management"]""",
        Context          = "Maria Santos calls in tears: her account was blocked and she cannot pay for her mother's urgent medical procedure ($1,200). She is demanding immediate unblocking. The system shows the block was placed 2 hours ago by the fraud team due to a suspected account takeover attempt.",
        LearnerRole      = "Branch Customer Service Officer",
        Task             = "Handle the situation with empathy while correctly applying the security protocol. Identify the appropriate response.",
        Explanation      = "A fraud block cannot be lifted without proper verification — bypassing it due to emotional pressure is a classic social engineering outcome. The correct approach: lead with empathy, explain the protection purpose, immediately connect to the fraud team for expedited review. If the caller is the legitimate account holder, the fraud team can fast-track. If not, the block protects the real customer.",
        Recommendation   = "Acknowledge the distress with genuine empathy. Explain the block protects her account. Do not lift the block yourself. Transfer to the fraud team with urgent escalation for expedited identity verification. Offer to stay on the line. If medical emergency is genuine, the fraud team has authority to fast-track after verification.",
        Evidence         = """[{"id":"e1","label":"Account Block Details","detail":"Account blocked: 2 hours ago. Blocking team: Fraud Prevention. Reason: Suspected ATO — 4 failed login attempts from new device followed by successful login, immediate large transfer attempt ($3,400 to new beneficiary)."},{"id":"e2","label":"Customer Statement","detail":"'Please, my mother needs surgery today. I have no other money. You have no right to block my account. Please just unblock it — I'll prove everything later. Please!' (customer is audibly crying)"},{"id":"e3","label":"Expedited Review Option","detail":"Fraud team SLA for urgent review: 15 minutes with live caller. Option available: direct warm transfer to fraud analyst who can verify caller identity and potentially lift block within the call."}]""",
        RedFlagOptions   = """[{"id":"r1","label":"Block was placed due to suspected ATO — a large transfer attempt was made from a new device","correct":true},{"id":"r2","label":"Emotional distress and medical emergency urgency are common social engineering pressure tactics","correct":true},{"id":"r3","label":"Lifting the block without verification would expose the real account holder to loss if this is the attacker","correct":true},{"id":"r4","label":"The 15-minute expedited fraud review option provides a fast, secure path for a legitimate customer","correct":true},{"id":"r5","label":"The customer's emotional state confirms she is the legitimate account holder","correct":false}]""",
        DecisionOptions  = """[{"id":"d1","label":"Acknowledge distress, explain protection, warm-transfer to fraud team for expedited 15-min verification","correct":true},{"id":"d2","label":"Unblock the account immediately given the medical emergency — security can be reviewed later","correct":false},{"id":"d3","label":"Inform the customer the block cannot be lifted for 24–48 hours — standard fraud review period","correct":false},{"id":"d4","label":"Ask the customer to visit the branch with ID before any action can be taken","correct":false}]""",
    };

    private static FraudScenario CxDisabilityService() => new()
    {
        Title            = "Disability Service",
        Description      = "Apply inclusive service standards to assist an elderly customer with communication difficulties while maintaining transaction security.",
        FraudType        = FraudSimType.SocialEngineering,
        Difficulty       = FraudSimDifficulty.Intermediate,
        RiskLevel        = FraudSimRisk.Low,
        EstimatedMinutes = 10,
        PassScore        = 65,
        Skills           = """["Inclusive Service","Accessibility Standards","Vulnerable Customer Protection","Transaction Security"]""",
        Context          = "An elderly customer (estimated 80s) approaches the counter struggling to communicate. He speaks limited English, has a hearing aid (appears not working), and seems confused about his transaction. A younger man accompanying him is offering to 'translate' and handle everything, including knowing the customer's account number.",
        LearnerRole      = "Branch Service Officer",
        Task             = "Identify the correct inclusive service approach and any protective concerns regarding the accompanying individual.",
        Explanation      = "While accommodating the customer's communication needs, the accompanying individual's level of account knowledge (knowing the account number) and eagerness to control the transaction raises elder financial abuse concerns. Correct protocol: use professional interpreter services, speak directly to the customer, and conduct part of the interaction privately if possible.",
        Recommendation   = "Use the bank's telephone interpreter service for the customer's language. Speak directly to the customer — not through the companion. Conduct a brief private interaction with the customer to check for coercion. If a large or unusual transaction is requested, apply enhanced scrutiny and consider involving branch manager.",
        Evidence         = """[{"id":"e1","label":"Customer Observation","detail":"Elderly male, approximately 80 years old. Confused expression. Hearing aid visible — appears to have low battery (whistling). Communication attempts: limited English phrases. Appears to be trying to say 'money' and 'help.'"},{"id":"e2","label":"Companion Behavior","detail":"Younger male (40s) — states: 'He's my uncle, he doesn't speak English well. His account number is ****4892. He wants to withdraw $8,000 in cash.' Companion is physically close to the customer and answers before the customer can attempt communication."},{"id":"e3","label":"Account Profile","detail":"Account holder: Nguyen Van Thanh — 83 years old. Monthly pension income: $1,100. Account balance: $11,200. Largest prior cash withdrawal: $200. No prior power-of-attorney or authorized third party documented."}]""",
        RedFlagOptions   = """[{"id":"r1","label":"Companion knows the account number and controls all communication — limiting customer's independent expression","correct":true},{"id":"r2","label":"Requested withdrawal ($8,000) is 40× the customer's largest previous cash transaction ($200)","correct":true},{"id":"r3","label":"No documented power-of-attorney or authorized third party on the account","correct":true},{"id":"r4","label":"Customer appears to be trying to communicate independently — companion intercepts his attempts","correct":true},{"id":"r5","label":"The companion states he is the customer's nephew — family relationships are trusted","correct":false}]""",
        DecisionOptions  = """[{"id":"d1","label":"Use professional phone interpreter — speak directly to customer privately — apply enhanced scrutiny to the withdrawal","correct":true},{"id":"d2","label":"Allow companion to translate and assist — family members are presumed to act in best interest","correct":false},{"id":"d3","label":"Decline all service until a formally documented power-of-attorney is provided","correct":false},{"id":"d4","label":"Complete the transaction quickly to minimize the customer's distress in the branch","correct":false}]""",
    };

    private static FraudScenario CxInternalEscalation() => new()
    {
        Title            = "Internal Escalation Protocol",
        Description      = "Respond to a suspected internal fraud case involving systematic bypass of fraud controls by a branch employee.",
        FraudType        = FraudSimType.SocialEngineering,
        Difficulty       = FraudSimDifficulty.Advanced,
        RiskLevel        = FraudSimRisk.Medium,
        EstimatedMinutes = 15,
        PassScore        = 75,
        Skills           = """["Internal Fraud Detection","Escalation Protocol","Evidence Preservation","Whistleblower Protection"]""",
        Context          = "A branch manager reports that teller Daniel Webb has been systematically overriding fraud alerts for 6 specific customers over 3 months (23 bypasses). These customers are unknown to each other but share a connection to a local money transfer business. Total bypassed transaction value: $340,000.",
        LearnerRole      = "Internal Audit / Compliance Investigator",
        Task             = "Identify the correct internal escalation steps and evidence preservation actions. Do not tip off the suspected employee.",
        Explanation      = "Internal fraud investigations require strict protocol: preserve evidence first, escalate through compliance and HR simultaneously, do not confront the employee. Tipping off the subject allows evidence destruction and asset flight. The correct sequence: document and secure audit trail → notify Compliance and HR → legal review → law enforcement if warranted.",
        Recommendation   = "Immediately preserve all digital audit logs related to Daniel Webb's bypass activity (do not delete or modify). Escalate simultaneously to Compliance Officer and HR Director. Do not confront Daniel Webb or inform any branch staff. Engage legal counsel. Suspend system access without advance notice. Notify law enforcement after internal counsel review.",
        Evidence         = """[{"id":"e1","label":"Teller Override Log","detail":"Daniel Webb — 23 fraud alert overrides in 90 days. Bank average: 0.8 overrides/teller/month. Webb: 7.7/month. All 23 overrides: same 6 customers. Reason codes entered: 'customer verified' — no supporting documentation."},{"id":"e2","label":"Customer Network Analysis","detail":"6 customers share no known personal relationship but all have mobile numbers registered to the same address: '1847 Market St' — registered address of 'FastCash Money Transfer LLC.'"},{"id":"e3","label":"Manager's Observation","detail":"Branch manager noticed Webb has been taking extended breaks coinciding with the 6 customers' visits. Webb received unexplained cash deposits totaling $4,200 to his personal account this quarter."}]""",
        RedFlagOptions   = """[{"id":"r1","label":"Override frequency is 9.6× the branch average — statistically anomalous pattern","correct":true},{"id":"r2","label":"All 23 overrides involve only 6 customers linked to the same money transfer business","correct":true},{"id":"r3","label":"Override reason codes lack required supporting documentation","correct":true},{"id":"r4","label":"Employee received unexplained personal cash deposits during the same period","correct":true},{"id":"r5","label":"Branch manager is responsible for not detecting this pattern sooner","correct":false}]""",
        DecisionOptions  = """[{"id":"d1","label":"Preserve audit logs, escalate to Compliance + HR, suspend system access, engage legal — do not tip off employee","correct":true},{"id":"d2","label":"Confront Daniel Webb directly to give him a chance to explain before escalating","correct":false},{"id":"d3","label":"Call law enforcement immediately before notifying internal compliance","correct":false},{"id":"d4","label":"Continue monitoring for 30 more days to build a stronger case before acting","correct":false}]""",
    };

    // ── Builder ──────────────────────────────────────────────────────────────

    private static List<FraudScenario> BuildScenarios() =>
    [
        AmlSuspiciousTransaction(),
        AmlKycBeneficialOwnership(),
        AmlSanctionsScreening(),
        AmlPepRisk(),
        AmlSarWriting(),
        CyberPhishingTriage(),
        CyberSocAlertTriage(),
        CyberIncidentResponse(),
        CyberZeroTrustAccess(),
        CyberDeepfakeVoice(),
        FraudVelocityPattern(),
        FraudMuleAccount(),
        FraudCardSkimming(),
        FraudChargebackTriage(),
        FraudAiAnomaly(),
        CxDifficultCustomer(),
        CxAddressChange(),
        CxAccountBlock(),
        CxDisabilityService(),
        CxInternalEscalation(),
    ];
}
