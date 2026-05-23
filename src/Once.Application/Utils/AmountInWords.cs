using System.Text;

namespace Once.Application.Utils;

public static class AmountInWords
{
    private static readonly string[] Units = { "", "bir ", "ikki ", "uch ", "to'rt ", "besh ", "olti ", "yetti ", "sakkiz ", "to'qqiz " };
    private static readonly string[] Tens = { "", "o'n ", "yigirma ", "o'ttiz ", "qirq ", "ellik ", "oltmish ", "yetmish ", "sakson ", "to'qson " };

    private static readonly string[] Thousands = { "yuz ", "ming ", "million ", "milliard ", "trillion ", "kvadrillion ", "kvintillion ", "sekstillion ", "septillion ", "oktillion " };
    private static readonly decimal[] ThousandsInNum = { 100, 1_000, 1_000_000, 1_000_000_000, 1_000_000_000_000, 1_000_000_000_000_000, 1_000_000_000_000_000_000, 1_000_000_000_000_000_000_000M, 1_000_000_000_000_000_000_000_000M };

    public static string Convert(decimal amount)
    {
        var isNegative = amount < 0;
        if (isNegative)
            amount = Math.Abs(amount);

        decimal integerPart = Math.Floor(amount);
        decimal fractionalPart = Math.Round((amount - integerPart) * 10000);

        var res = new StringBuilder();

        if (integerPart == 0)
        {
            res.Append("nol so'm ");
        }
        else
        {
            res.Append(NumToWord(integerPart));
            res.Append("so'm ");
        }

        if (fractionalPart != 0)
        {
            res.Append(NumToWord(fractionalPart));
            res.Append("tiyin");
        }

        if (isNegative)
            res.Insert(0, "minus ");

        return res.ToString().Trim();
    }

    private static StringBuilder NumToWord(decimal amount)
    {
        var res = new StringBuilder();

        for (int i = ThousandsInNum.Length - 1; i >= 0; i--)
            if (amount >= ThousandsInNum[i])
            {
                res.Append(NumToWord(amount/ ThousandsInNum[i]));
                res.Append(Thousands[i]);
                res.Append(NumToWord(amount % ThousandsInNum[i]));

                return res;
            }

        res.Append(Tens[(int)(amount / 10)]);
        res.Append(Units[(int)(amount % 10)]);

        return res;
    }
}
