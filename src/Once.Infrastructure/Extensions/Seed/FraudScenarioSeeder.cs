using Microsoft.EntityFrameworkCore;
using Once.Domain.Entities;
using Once.Domain.Entities.Common;
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
        Title = new MultiLanguageField
        {
            En   = "Suspicious Transaction Detection",
            Uz   = "Shubhali tranzaksiyalarni aniqlash",
            Ru   = "Выявление подозрительных транзакций",
            Cyrl = "Шубҳали трансаксияларни аниқлаш",
        },
        Description = new MultiLanguageField
        {
            En   = "Analyze a series of structured cash transactions designed to evade regulatory reporting thresholds and determine the appropriate compliance response.",
            Uz   = "Tartibga solish bo'yicha hisobot chegaralaridan qochish uchun mo'ljallangan tuzilmali naqd pul tranzaksiyalar seriyasini tahlil qiling va tegishli muvofiqlik javobini aniqlang.",
            Ru   = "Проанализируйте серию структурированных денежных операций, предназначенных для уклонения от порогов регуляторной отчётности, и определите надлежащий ответ по комплаенсу.",
            Cyrl = "Тартибга солиш бўйича ҳисобот чегараларидан қочиш учун мўлжалланган тузилмали нақд пул трансаксиялар серияcини таҳлил қилинг ва тегишли мувофиқлик жавобини аниқланг.",
        },
        FraudType        = FraudSimType.AmlKyc,
        Difficulty       = FraudSimDifficulty.Advanced,
        RiskLevel        = FraudSimRisk.High,
        EstimatedMinutes = 15,
        PassScore        = 75,
        LearnerRole = new MultiLanguageField
        {
            En   = "AML Compliance Officer at National Bank",
            Uz   = "Milliy bank AML muvofiqlik xodimi",
            Ru   = "Специалист по AML-комплаенсу Национального банка",
            Cyrl = "Миллий банк AML мувофиқлик ходими",
        },
    };

    private static FraudScenario AmlKycBeneficialOwnership() => new()
    {
        Title = new MultiLanguageField
        {
            En   = "KYC Mastery – Beneficial Ownership",
            Uz   = "KYC mahorati – foydalanuvchi egasi",
            Ru   = "Мастерство KYC – бенефициарное владение",
            Cyrl = "KYC маҳорати – фойдаланувчи эгаси",
        },
        Description = new MultiLanguageField
        {
            En   = "Navigate a complex corporate ownership structure to identify the ultimate beneficial owners and assess hidden PEP exposure.",
            Uz   = "Yakuniy benefitsiar egalarini aniqlash va yashirin PEP ta'sirini baholash uchun murakkab korporativ egalik tuzilmasini o'rganing.",
            Ru   = "Изучите сложную корпоративную структуру собственности для выявления конечных бенефициарных владельцев и оценки скрытого PEP-воздействия.",
            Cyrl = "Якуний бенефитсиар эгаларини аниқлаш ва яширин PEP таъсирини баҳолаш учун мураккаб корпоратив эгалик тузилмасини ўрганинг.",
        },
        FraudType        = FraudSimType.AmlKyc,
        Difficulty       = FraudSimDifficulty.Intermediate,
        RiskLevel        = FraudSimRisk.High,
        EstimatedMinutes = 12,
        PassScore        = 70,
        LearnerRole = new MultiLanguageField
        {
            En   = "KYC Analyst at the bank's onboarding team",
            Uz   = "Bank onboarding jamoasidagi KYC analitik",
            Ru   = "KYC-аналитик отдела онбординга банка",
            Cyrl = "Банк онбординг жамоасидаги KYC аналитик",
        },
    };

    private static FraudScenario AmlSanctionsScreening() => new()
    {
        Title = new MultiLanguageField
        {
            En   = "Sanctions Screening (OFAC/UN)",
            Uz   = "Sanksiyalarni tekshirish (OFAC/BMT)",
            Ru   = "Проверка санкций (OFAC/ООН)",
            Cyrl = "Санксияларни текшириш (OFAC/БМТ)",
        },
        Description = new MultiLanguageField
        {
            En   = "Evaluate a sanctions screening alert on an incoming wire transfer and determine whether it is a true match or false positive.",
            Uz   = "Kiruvchi pul o'tkazmasi bo'yicha sanksiya tekshiruvi ogohlantirishini baholang va bu haqiqiy moslik yoki soxta ijobiy ekanligini aniqlang.",
            Ru   = "Оцените оповещение о проверке санкций по входящему банковскому переводу и определите, является ли это истинным совпадением или ложным срабатыванием.",
            Cyrl = "Кирувчи пул ўтказмаси бўйича санксия текшируви огоҳлантиришини баҳоланг ва бу ҳақиқий мослик ёки сохта ижобий эканлигини аниқланг.",
        },
        FraudType        = FraudSimType.AmlKyc,
        Difficulty       = FraudSimDifficulty.Advanced,
        RiskLevel        = FraudSimRisk.High,
        EstimatedMinutes = 15,
        PassScore        = 80,
        LearnerRole = new MultiLanguageField
        {
            En   = "Payments Compliance Analyst",
            Uz   = "To'lovlar muvofiqlik analitiki",
            Ru   = "Аналитик по комплаенсу платежей",
            Cyrl = "Тўловлар мувофиқлик аналитики",
        },
    };

    private static FraudScenario AmlPepRisk() => new()
    {
        Title = new MultiLanguageField
        {
            En   = "PEP Risk Assessment",
            Uz   = "PEP riskini baholash",
            Ru   = "Оценка рисков PEP",
            Cyrl = "PEP рискини баҳолаш",
        },
        Description = new MultiLanguageField
        {
            En   = "Identify politically exposed person (PEP) risk in a new account application and apply the correct enhanced due diligence protocol.",
            Uz   = "Yangi hisob ochish arizasida siyosiy ta'sir o'tkazuvchi shaxs (PEP) riskini aniqlang va to'g'ri kuchaytrilgan tekshiruv protokolini qo'llang.",
            Ru   = "Выявите риск политически значимого лица (PEP) в заявке на открытие нового счёта и примените правильный протокол расширенной проверки.",
            Cyrl = "Янги ҳисоб очиш аризасида сиёсий таъсир ўтказувчи шахс (PEP) рискини аниқланг ва тўғри кучайтрилган текширув протоколини қўлланг.",
        },
        FraudType        = FraudSimType.AmlKyc,
        Difficulty       = FraudSimDifficulty.Intermediate,
        RiskLevel        = FraudSimRisk.High,
        EstimatedMinutes = 12,
        PassScore        = 70,
        LearnerRole = new MultiLanguageField
        {
            En   = "Private Banking Relationship Manager",
            Uz   = "Xususiy bank munosabatlari menejeri",
            Ru   = "Менеджер по работе с клиентами private banking",
            Cyrl = "Хусусий банк муносабатлари менежери",
        },
    };

    private static FraudScenario AmlSarWriting() => new()
    {
        Title = new MultiLanguageField
        {
            En   = "SAR Writing",
            Uz   = "SAR yozish",
            Ru   = "Написание SAR",
            Cyrl = "SAR ёзиш",
        },
        Description = new MultiLanguageField
        {
            En   = "Construct a complete and legally compliant Suspicious Activity Report for a confirmed money laundering case.",
            Uz   = "Tasdiqlangan pul yuvish holati uchun to'liq va huquqiy jihatdan muvofiq Shubhali Faoliyat Hisoboti tuzing.",
            Ru   = "Составьте полный и юридически корректный Отчёт о подозрительной деятельности для подтверждённого случая отмывания денег.",
            Cyrl = "Тасдиқланган пул ювиш ҳолати учун тўлиқ ва ҳуқуқий жиҳатдан мувофиқ Шубҳали Фаолият Ҳисоботи тузинг.",
        },
        FraudType        = FraudSimType.AmlKyc,
        Difficulty       = FraudSimDifficulty.Advanced,
        RiskLevel        = FraudSimRisk.High,
        EstimatedMinutes = 15,
        PassScore        = 75,
        LearnerRole = new MultiLanguageField
        {
            En   = "BSA/AML Officer",
            Uz   = "BSA/AML xodimi",
            Ru   = "Офицер BSA/AML",
            Cyrl = "BSA/AML ходими",
        },
    };

    // ── Cybersecurity / InfoSec ──────────────────────────────────────────────

    private static FraudScenario CyberPhishingTriage() => new()
    {
        Title = new MultiLanguageField
        {
            En   = "Phishing Triage",
            Uz   = "Fishing triyaj",
            Ru   = "Триаж фишинга",
            Cyrl = "Фишинг триаж",
        },
        Description = new MultiLanguageField
        {
            En   = "Analyze a suspicious email reported by an employee and determine whether it is a phishing attempt requiring escalation.",
            Uz   = "Xodim tomonidan xabar qilingan shubhali elektron pochtani tahlil qiling va bu eskalatsiya talab qiladigan fishing urinishi ekanligini aniqlang.",
            Ru   = "Проанализируйте подозрительное электронное письмо, о котором сообщил сотрудник, и определите, является ли оно попыткой фишинга, требующей эскалации.",
            Cyrl = "Ходим томонидан хабар қилинган шубҳали электрон поштани таҳлил қилинг ва бу эскалация талаб қиладиган фишинг уринishi эканлигини аниқланг.",
        },
        FraudType        = FraudSimType.Phishing,
        Difficulty       = FraudSimDifficulty.Beginner,
        RiskLevel        = FraudSimRisk.Medium,
        EstimatedMinutes = 8,
        PassScore        = 60,
        LearnerRole = new MultiLanguageField
        {
            En   = "Security Analyst (Tier 1 SOC)",
            Uz   = "Xavfsizlik analitigi (1-darajali SOC)",
            Ru   = "Аналитик безопасности (Tier 1 SOC)",
            Cyrl = "Хавфсизлик аналитиги (1-даражали SOC)",
        },
    };

    private static FraudScenario CyberSocAlertTriage() => new()
    {
        Title = new MultiLanguageField
        {
            En   = "SOC Alert Triage",
            Uz   = "SOC ogohlantirishini saralash",
            Ru   = "Триаж оповещений SOC",
            Cyrl = "SOC огоҳлантиришини саралаш",
        },
        Description = new MultiLanguageField
        {
            En   = "Triage a SIEM alert for a successful login from an anomalous location following multiple failed attempts and determine severity and next steps.",
            Uz   = "Bir nechta muvaffaqiyatsiz urinishlardan keyin anomal joydan muvaffaqiyatli kirish uchun SIEM ogohlantirishini saralang va og'irlik va keyingi qadamlarni aniqlang.",
            Ru   = "Выполните триаж оповещения SIEM об успешном входе из аномального места после нескольких неудачных попыток и определите серьёзность и дальнейшие шаги.",
            Cyrl = "Бир неча муваффақиятсиз уринишлардан кейин аномал жойдан муваффақиятли кириш учун SIEM огоҳлантиришини саралинг ва оғирлик ва кейинги қадамларни аниқланг.",
        },
        FraudType        = FraudSimType.Phishing,
        Difficulty       = FraudSimDifficulty.Intermediate,
        RiskLevel        = FraudSimRisk.Medium,
        EstimatedMinutes = 12,
        PassScore        = 65,
        LearnerRole = new MultiLanguageField
        {
            En   = "SOC Analyst (Level 2)",
            Uz   = "SOC analitigi (2-daraja)",
            Ru   = "Аналитик SOC (уровень 2)",
            Cyrl = "SOC аналитиги (2-даража)",
        },
    };

    private static FraudScenario CyberIncidentResponse() => new()
    {
        Title = new MultiLanguageField
        {
            En   = "Incident Response (NIST)",
            Uz   = "Voqeaga javob (NIST)",
            Ru   = "Реагирование на инциденты (NIST)",
            Cyrl = "Воқеага жавоб (NIST)",
        },
        Description = new MultiLanguageField
        {
            En   = "Lead the response to a ransomware attack on the bank's network using the NIST Incident Response Framework.",
            Uz   = "NIST Voqeaga Javob Berish Doirasi yordamida bank tarmog'iga ransomware hujumiga javobni boshqaring.",
            Ru   = "Руководите реагированием на атаку программы-вымогателя на сеть банка с использованием системы реагирования на инциденты NIST.",
            Cyrl = "NIST Воқеага Жавоб Бериш Доираси ёрдамида банк тармоғига ransomware ҳужумига жавобни бошқаринг.",
        },
        FraudType        = FraudSimType.Phishing,
        Difficulty       = FraudSimDifficulty.Advanced,
        RiskLevel        = FraudSimRisk.High,
        EstimatedMinutes = 15,
        PassScore        = 75,
        LearnerRole = new MultiLanguageField
        {
            En   = "Incident Response Lead",
            Uz   = "Voqeaga javob berish rahbari",
            Ru   = "Руководитель группы реагирования на инциденты",
            Cyrl = "Воқеага жавоб бериш раҳбари",
        },
    };

    private static FraudScenario CyberZeroTrustAccess() => new()
    {
        Title = new MultiLanguageField
        {
            En   = "Zero Trust Access",
            Uz   = "Zero Trust kirish",
            Ru   = "Доступ по принципу Zero Trust",
            Cyrl = "Zero Trust кириш",
        },
        Description = new MultiLanguageField
        {
            En   = "Evaluate an emergency access request against Zero Trust principles and determine whether to grant access from an unmanaged device.",
            Uz   = "Favqulodda kirish so'rovini Zero Trust tamoyillariga nisbatan baholang va boshqarilmaydigan qurilmadan kirishga ruxsat berish-bermaslikni aniqlang.",
            Ru   = "Оцените запрос экстренного доступа в соответствии с принципами Zero Trust и определите, следует ли предоставить доступ с неуправляемого устройства.",
            Cyrl = "Фавқулодда кириш сўровини Zero Trust тамойилларига нисбатан баҳоланг ва бошқарилмайдиган қурилмадан киришга рухсат бериш-бермасликни аниқланг.",
        },
        FraudType        = FraudSimType.Phishing,
        Difficulty       = FraudSimDifficulty.Intermediate,
        RiskLevel        = FraudSimRisk.Medium,
        EstimatedMinutes = 10,
        PassScore        = 65,
        LearnerRole = new MultiLanguageField
        {
            En   = "IT Security Manager",
            Uz   = "IT xavfsizlik menejeri",
            Ru   = "Менеджер по IT-безопасности",
            Cyrl = "IT хавфсизлик менежери",
        },
    };

    private static FraudScenario CyberDeepfakeVoice() => new()
    {
        Title = new MultiLanguageField
        {
            En   = "Deepfake Voice Verification",
            Uz   = "Deepfake ovozni tekshirish",
            Ru   = "Верификация голоса при deepfake-атаке",
            Cyrl = "Deepfake овозни текшириш",
        },
        Description = new MultiLanguageField
        {
            En   = "Detect deepfake voice fraud in a high-value wire transfer call and apply the correct multi-factor verification protocol.",
            Uz   = "Katta miqdordagi pul o'tkazmasi qo'ng'irog'ida deepfake ovoz firibgarligini aniqlang va to'g'ri ko'p faktorli tekshiruv protokolini qo'llang.",
            Ru   = "Обнаружьте мошенничество с использованием deepfake-голоса в звонке с крупным банковским переводом и примените правильный протокол многофакторной проверки.",
            Cyrl = "Катта миқдордаги пул ўтказмаси қўнғироғида deepfake овоз фирибгарлигини аниқланг ва тўғри кўп факторли текширув протоколини қўлланг.",
        },
        FraudType        = FraudSimType.DeepfakeCall,
        Difficulty       = FraudSimDifficulty.Advanced,
        RiskLevel        = FraudSimRisk.High,
        EstimatedMinutes = 12,
        PassScore        = 75,
        LearnerRole = new MultiLanguageField
        {
            En   = "Customer Service Security Specialist",
            Uz   = "Mijozlarga xizmat ko'rsatish xavfsizlik mutaxassisi",
            Ru   = "Специалист по безопасности в обслуживании клиентов",
            Cyrl = "Мижозларга хизмат кўрсатиш хавфсизлик мутахассиси",
        },
    };

    // ── Fraud Monitoring ─────────────────────────────────────────────────────

    private static FraudScenario FraudVelocityPattern() => new()
    {
        Title = new MultiLanguageField
        {
            En   = "Velocity Pattern Detection",
            Uz   = "Tezlik naqshini aniqlash",
            Ru   = "Обнаружение паттернов высокой частоты",
            Cyrl = "Тезлик нақшини аниқлаш",
        },
        Description = new MultiLanguageField
        {
            En   = "Analyze a high-velocity debit card transaction pattern on a pensioner's account to determine if it represents fraud.",
            Uz   = "Pensioner hisobidagi yuqori tezlikdagi debet karta tranzaksiyalar naqshini tahlil qilib, bu firibgarlikni ifodalaydi-yo'qligini aniqlang.",
            Ru   = "Проанализируйте паттерн высокочастотных операций по дебетовой карте на счёте пенсионера и определите, является ли это мошенничеством.",
            Cyrl = "Пенсионер ҳисобидаги юқори тезликдаги дебет карта трансаксиялар нақшини таҳлил қилиб, бу фирибгарликни ифодалайди-йўқлигини аниқланг.",
        },
        FraudType        = FraudSimType.Transaction,
        Difficulty       = FraudSimDifficulty.Intermediate,
        RiskLevel        = FraudSimRisk.High,
        EstimatedMinutes = 12,
        PassScore        = 70,
        LearnerRole = new MultiLanguageField
        {
            En   = "Fraud Analyst",
            Uz   = "Firibgarlik analitiki",
            Ru   = "Аналитик по мошенничеству",
            Cyrl = "Фирибгарлик аналитики",
        },
    };

    private static FraudScenario FraudMuleAccount() => new()
    {
        Title = new MultiLanguageField
        {
            En   = "Mule Account Spotting",
            Uz   = "Qo'g'irchoq hisobni aniqlash",
            Ru   = "Выявление счетов-дропов",
            Cyrl = "Қўғирчоқ ҳисобни аниқлаш",
        },
        Description = new MultiLanguageField
        {
            En   = "Identify money mule account indicators from transaction patterns and customer behavior, and take appropriate action.",
            Uz   = "Tranzaksiya naqshlari va mijoz xulq-atvoridan pul qo'g'irchoq hisob ko'rsatkichlarini aniqlang va tegishli chora ko'ring.",
            Ru   = "Определите признаки счёта денежного мула по паттернам транзакций и поведению клиента и примите надлежащие меры.",
            Cyrl = "Трансаксия нақшлари ва мижоз хулқ-атворидан пул қўғирчоқ ҳисоб кўрсаткичларини аниқланг ва тегишли чора кўринг.",
        },
        FraudType        = FraudSimType.Transaction,
        Difficulty       = FraudSimDifficulty.Advanced,
        RiskLevel        = FraudSimRisk.High,
        EstimatedMinutes = 15,
        PassScore        = 75,
        LearnerRole = new MultiLanguageField
        {
            En   = "Fraud Investigator",
            Uz   = "Firibgarlikni tekshiruvchi",
            Ru   = "Следователь по мошенничеству",
            Cyrl = "Фирибгарликни текширувчи",
        },
    };

    private static FraudScenario FraudCardSkimming() => new()
    {
        Title = new MultiLanguageField
        {
            En   = "Card Skimming Behavioral",
            Uz   = "Karta skimming xulq-atvori",
            Ru   = "Поведение при скимминге карт",
            Cyrl = "Карта скимминг хулқ-атвори",
        },
        Description = new MultiLanguageField
        {
            En   = "Investigate an ATM card skimming incident, identify compromised cards, and prioritize actions to minimize fraud losses.",
            Uz   = "ATM karta skimming voqeasini tekshiring, buzilgan kartalarni aniqlang va firibgarlik yo'qotishlarini minimallashtirish uchun harakatlarni ustuvorlashtiring.",
            Ru   = "Расследуйте инцидент со скиммингом банкоматной карты, выявите скомпрометированные карты и расставьте приоритеты действий для минимизации потерь от мошенничества.",
            Cyrl = "ATM карта скимминг воқеасини текширинг, бузилган карталарни аниқланг ва фирибгарлик йўқотишларини минималлаштириш учун ҳаракатларни устуворлаштиринг.",
        },
        FraudType        = FraudSimType.Transaction,
        Difficulty       = FraudSimDifficulty.Intermediate,
        RiskLevel        = FraudSimRisk.Medium,
        EstimatedMinutes = 10,
        PassScore        = 65,
        LearnerRole = new MultiLanguageField
        {
            En   = "Fraud Operations Specialist",
            Uz   = "Firibgarlik operatsiyalari mutaxassisi",
            Ru   = "Специалист по операциям с мошенничеством",
            Cyrl = "Фирибгарлик операциялари мутахассиси",
        },
    };

    private static FraudScenario FraudChargebackTriage() => new()
    {
        Title = new MultiLanguageField
        {
            En   = "Chargeback Triage",
            Uz   = "Qaytarma triyaji",
            Ru   = "Триаж чарджбэков",
            Cyrl = "Қайтарма триажи",
        },
        Description = new MultiLanguageField
        {
            En   = "Evaluate a dispute claim against authentication evidence to determine whether to accept or deny a chargeback.",
            Uz   = "Qaytarmani qabul qilish yoki rad etishni aniqlash uchun autentifikatsiya dalillariga qarshi da'voni baholang.",
            Ru   = "Оцените претензию на возврат платежа в сравнении с доказательствами аутентификации и определите, принять или отклонить чарджбэк.",
            Cyrl = "Қайтармани қабул қилиш ёки рад этишни аниқлаш учун аутентификация далилларига қарши даъвони баҳоланг.",
        },
        FraudType        = FraudSimType.Transaction,
        Difficulty       = FraudSimDifficulty.Beginner,
        RiskLevel        = FraudSimRisk.Medium,
        EstimatedMinutes = 8,
        PassScore        = 60,
        LearnerRole = new MultiLanguageField
        {
            En   = "Dispute Resolution Specialist",
            Uz   = "Nizolarni hal qilish mutaxassisi",
            Ru   = "Специалист по урегулированию споров",
            Cyrl = "Низоларни ҳал қилиш мутахассиси",
        },
    };

    private static FraudScenario FraudAiAnomaly() => new()
    {
        Title = new MultiLanguageField
        {
            En   = "AI Anomaly Tuning",
            Uz   = "AI anomaliyasini sozlash",
            Ru   = "Настройка аномалий ИИ",
            Cyrl = "AI аномалиясини созлаш",
        },
        Description = new MultiLanguageField
        {
            En   = "Diagnose a machine learning fraud model's high false positive rate and recommend targeted model improvements.",
            Uz   = "Mashinani o'rganish firibgarlik modelining yuqori soxta ijobiy ko'rsatkichini tashxislang va maqsadli model yaxshilanishlarini tavsiya qiling.",
            Ru   = "Диагностируйте высокий процент ложных срабатываний модели машинного обучения для выявления мошенничества и рекомендуйте целевые улучшения модели.",
            Cyrl = "Машинани ўрганиш фирибгарлик моделининг юқори сохта ижобий кўрсаткичини ташхисланг ва мақсадли модел яхшиланишларини тавсия қилинг.",
        },
        FraudType        = FraudSimType.Transaction,
        Difficulty       = FraudSimDifficulty.Advanced,
        RiskLevel        = FraudSimRisk.High,
        EstimatedMinutes = 15,
        PassScore        = 75,
        LearnerRole = new MultiLanguageField
        {
            En   = "Fraud Data Scientist",
            Uz   = "Firibgarlik ma'lumotlar olimi",
            Ru   = "Дата-сайентист по мошенничеству",
            Cyrl = "Фирибгарлик маълумотлар олими",
        },
    };

    // ── Customer Support / Front Office ──────────────────────────────────────

    private static FraudScenario CxDifficultCustomer() => new()
    {
        Title = new MultiLanguageField
        {
            En   = "Difficult-Customer De-escalation",
            Uz   = "Qiyin mijozni tinchlantirish",
            Ru   = "Деэскалация конфликта с трудным клиентом",
            Cyrl = "Қийин мижозни тинчлантириш",
        },
        Description = new MultiLanguageField
        {
            En   = "De-escalate an angry customer threatening regulatory action after an erroneous fee charge while preserving the banking relationship.",
            Uz   = "Bank munosabatlarini saqlab qolgan holda noto'g'ri to'lov undirilgandan keyin tartibga solish choralarini tahdid qilayotgan g'azablangan mijozni tinchlantiring.",
            Ru   = "Урегулируйте конфликт с разгневанным клиентом, угрожающим регуляторными мерами после ошибочного списания комиссии, сохранив при этом банковские отношения.",
            Cyrl = "Банк муносабатларини сақлаб қолган ҳолда нотўғри тўлов ундирилгандан кейин тартибга солиш чораларини таҳдид қилаётган ғазабланган мижозни тинчлантиринг.",
        },
        FraudType        = FraudSimType.SocialEngineering,
        Difficulty       = FraudSimDifficulty.Beginner,
        RiskLevel        = FraudSimRisk.Low,
        EstimatedMinutes = 8,
        PassScore        = 60,
        LearnerRole = new MultiLanguageField
        {
            En   = "Customer Service Representative",
            Uz   = "Mijozlarga xizmat ko'rsatish vakili",
            Ru   = "Представитель службы поддержки клиентов",
            Cyrl = "Мижозларга хизмат кўрсатиш вакили",
        },
    };

    private static FraudScenario CxAddressChange() => new()
    {
        Title = new MultiLanguageField
        {
            En   = "Address-Change Verification",
            Uz   = "Manzilni o'zgartirishni tasdiqlash",
            Ru   = "Верификация смены адреса",
            Cyrl = "Манзилни ўзгартиришни тасдиқлаш",
        },
        Description = new MultiLanguageField
        {
            En   = "Apply social engineering detection and verification protocol during a caller's account address change request.",
            Uz   = "Qo'ng'iroq qiluvchining hisob manzilini o'zgartirish so'rovi paytida ijtimoiy muhandislik aniqlash va tekshiruv protokolini qo'llang.",
            Ru   = "Примените протокол обнаружения социальной инженерии и верификации во время запроса звонящего на изменение адреса аккаунта.",
            Cyrl = "Қўнғироқ қилувчининг ҳисоб манзилини ўзгартириш сўрови пайтида ижтимоий муҳандислик аниқлаш ва текшируш протоколини қўлланг.",
        },
        FraudType        = FraudSimType.SocialEngineering,
        Difficulty       = FraudSimDifficulty.Intermediate,
        RiskLevel        = FraudSimRisk.Medium,
        EstimatedMinutes = 10,
        PassScore        = 65,
        LearnerRole = new MultiLanguageField
        {
            En   = "Call Center Security Agent",
            Uz   = "Call center xavfsizlik agenti",
            Ru   = "Агент безопасности колл-центра",
            Cyrl = "Call center хавфсизлик агенти",
        },
    };

    private static FraudScenario CxAccountBlock() => new()
    {
        Title = new MultiLanguageField
        {
            En   = "Account Block Empathy",
            Uz   = "Hisob blokida empatiya",
            Ru   = "Эмпатия при блокировке счёта",
            Cyrl = "Ҳисоб блокида эмпатия",
        },
        Description = new MultiLanguageField
        {
            En   = "Handle an emotionally distressed customer whose account is blocked due to a fraud suspicion, balancing empathy with security protocol.",
            Uz   = "Firibgarlik shubhasi tufayli hisobi bloklangan hissiyotli xafa mijozni empatiya va xavfsizlik protokoli o'rtasida muvozanatni saqlagan holda boshqaring.",
            Ru   = "Работайте с эмоционально расстроенным клиентом, чей счёт заблокирован из-за подозрения в мошенничестве, соблюдая баланс между эмпатией и протоколом безопасности.",
            Cyrl = "Фирибгарлик шубҳаси туфайли ҳисоби блокланган ҳиссиётли хафа мижозни эмпатия ва хавфсизлик протоколи ўртасида мувозанатни сақлаган ҳолда бошқаринг.",
        },
        FraudType        = FraudSimType.SocialEngineering,
        Difficulty       = FraudSimDifficulty.Intermediate,
        RiskLevel        = FraudSimRisk.Medium,
        EstimatedMinutes = 10,
        PassScore        = 65,
        LearnerRole = new MultiLanguageField
        {
            En   = "Branch Customer Service Officer",
            Uz   = "Filial mijozlarga xizmat ko'rsatish xodimi",
            Ru   = "Офицер по работе с клиентами филиала",
            Cyrl = "Филиал мижозларга хизмат кўрсатиш ходими",
        },
    };

    private static FraudScenario CxDisabilityService() => new()
    {
        Title = new MultiLanguageField
        {
            En   = "Disability Service",
            Uz   = "Nogironlik xizmati",
            Ru   = "Обслуживание клиентов с ограниченными возможностями",
            Cyrl = "Ногиронлик хизмати",
        },
        Description = new MultiLanguageField
        {
            En   = "Apply inclusive service standards to assist an elderly customer with communication difficulties while maintaining transaction security.",
            Uz   = "Tranzaksiya xavfsizligini saqlab qolgan holda muloqot qiyinchiliklariga duch keladigan keksa mijozga yordam berish uchun inklyuziv xizmat standartlarini qo'llang.",
            Ru   = "Применяйте инклюзивные стандарты обслуживания для помощи пожилому клиенту с трудностями общения, сохраняя при этом безопасность транзакций.",
            Cyrl = "Трансаксия хавфсизлигини сақлаб қолган ҳолда мулоқот қийинчиликларига дуч келадиган кекса мижозга ёрдам бериш учун инклюзив хизмат стандартларини қўлланг.",
        },
        FraudType        = FraudSimType.SocialEngineering,
        Difficulty       = FraudSimDifficulty.Intermediate,
        RiskLevel        = FraudSimRisk.Low,
        EstimatedMinutes = 10,
        PassScore        = 65,
        LearnerRole = new MultiLanguageField
        {
            En   = "Branch Service Officer",
            Uz   = "Filial xizmat xodimi",
            Ru   = "Офицер обслуживания филиала",
            Cyrl = "Филиал хизмат ходими",
        },
    };

    private static FraudScenario CxInternalEscalation() => new()
    {
        Title = new MultiLanguageField
        {
            En   = "Internal Escalation Protocol",
            Uz   = "Ichki eskalatsiya protokoli",
            Ru   = "Протокол внутренней эскалации",
            Cyrl = "Ички эскалация протоколи",
        },
        Description = new MultiLanguageField
        {
            En   = "Respond to a suspected internal fraud case involving systematic bypass of fraud controls by a branch employee.",
            Uz   = "Filial xodimi tomonidan firibgarlik nazoratini tizimli ravishda chetlab o'tishni o'z ichiga olgan gumonlanayotgan ichki firibgarlik holatiga javob bering.",
            Ru   = "Реагируйте на предполагаемый случай внутреннего мошенничества, связанный с систематическим обходом средств контроля мошенничества сотрудником филиала.",
            Cyrl = "Филиал ходими томонидан фирибгарлик назоратини тизимли равишда четлаб ўтишни ўз ичига олган гумонланаётган ички фирибгарлик ҳолатига жавоб беринг.",
        },
        FraudType        = FraudSimType.SocialEngineering,
        Difficulty       = FraudSimDifficulty.Advanced,
        RiskLevel        = FraudSimRisk.Medium,
        EstimatedMinutes = 15,
        PassScore        = 75,
        LearnerRole = new MultiLanguageField
        {
            En   = "Internal Audit / Compliance Investigator",
            Uz   = "Ichki audit / muvofiqlik tergovchisi",
            Ru   = "Внутренний аудитор / следователь по комплаенсу",
            Cyrl = "Ички аудит / мувофиқлик терговчиси",
        },
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
