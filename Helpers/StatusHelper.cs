using SMARTV3.Models;

namespace SMARTV3.Helpers
{
    public class StatusHelper
    {
        private SMARTV3DbContext _context;
        private readonly PetsoverallStatus GreenStatus;
        private readonly PetsoverallStatus YellowStatus;
        private readonly PetsoverallStatus OrangeStatus;
        private readonly PetsoverallStatus RedStatus;
        private static readonly string White = "white";

        public StatusHelper(SMARTV3DbContext context)
        {
            _context = context;
            GreenStatus = _context.PetsoverallStatuses.Where(status => status.Id == 1).First();
            YellowStatus = _context.PetsoverallStatuses.Where(status => status.Id == 2).First();
            OrangeStatus = _context.PetsoverallStatuses.Where(status => status.Id == 3).First();
            RedStatus = _context.PetsoverallStatuses.Where(status => status.Id == 4).First();
        }

        public CardStatus CalculateStatus(string selectedStatuses, int selectedStatusCount)
        {
            CardStatus cardStatus = new();

            // If we have a count of five selections
            switch (selectedStatusCount)
            {
                case 6:
                    cardStatus = CalculateSixStatuses(selectedStatuses);
                    break;

                case 5:
                    cardStatus = CalculateFiveStatuses(selectedStatuses);
                    break;

                case 4:
                    cardStatus = CalculateFourStatuses(selectedStatuses);
                    break;

                case 3:
                    cardStatus = CalculateThreeStatuses(selectedStatuses);
                    break;

                case 2:
                    cardStatus = CalculateTwoStatuses(selectedStatuses);
                    break;

                case 1:
                    cardStatus = CalculateOneStatuses(selectedStatuses);
                    break;

                case 0:
                    cardStatus.statusID = 5;
                    cardStatus.statusColour = White;
                    break;
            }

            return cardStatus;
        }

        private CardStatus CalculateSixStatuses(string selectedStatuses)
        {
            CardStatus cardStatus = new();
            switch (selectedStatuses)
            {
                // 6 green
                case "6000":
                    cardStatus.statusColour = GreenStatus.StatusDisplayColour!;
                    cardStatus.statusID = 1;
                    break;

                // 5 green one yellow
                case "5100":
                    cardStatus.statusColour = GreenStatus.StatusDisplayColour!;
                    cardStatus.statusID = 1;
                    break;

                // 5 green one orange
                case "5010":
                    cardStatus.statusColour = YellowStatus.StatusDisplayColour!;
                    cardStatus.statusID = 2;
                    break;
                // 5 green one red
                case "5001":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;

                // 4 Green 2 Yellow
                case "4200":
                    cardStatus.statusColour = YellowStatus.StatusDisplayColour!;
                    cardStatus.statusID = 2;
                    break;

                // 4 Green 1 yellow 1 Orange
                case "4110":
                    cardStatus.statusColour = YellowStatus.StatusDisplayColour!;
                    cardStatus.statusID = 2;
                    break;
                // 4 Green 1 yellow 1 red
                case "4101":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;

                // 4 Green 2 Orange
                case "4020":
                    cardStatus.statusColour = YellowStatus.StatusDisplayColour!;
                    cardStatus.statusID = 2;
                    break;
                // 4 Green 1 orange 1 red
                case "4011":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 4 Green 2 red
                case "4002":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;

                // 3 Green 3 yellow
                case "3300":
                    cardStatus.statusColour = YellowStatus.StatusDisplayColour!;
                    cardStatus.statusID = 2;
                    break;

                // 3 Green 2 yellow 1 orange
                case "3210":
                    cardStatus.statusColour = YellowStatus.StatusDisplayColour!;
                    cardStatus.statusID = 2;
                    break;
                // 3 Green 2 yellow 1 red
                case "3201":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;

                // 3 Green 1 yellow 2 orange
                case "3120":
                    cardStatus.statusColour = YellowStatus.StatusDisplayColour!;
                    cardStatus.statusID = 2;
                    break;
                // 3 Green 1 yellow 1 orange 1 red
                case "3111":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 3 Green 1 yellow 2 red
                case "3102":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;

                // 3 Green 3 orange
                case "3030":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 3 Green 2 orange 1 red
                case "3021":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 3 Green 1 orange 2 red
                case "3012":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 3 Green 3 red
                case "3003":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;

                // 2 Green 4 yellow
                case "2400":
                    cardStatus.statusColour = YellowStatus.StatusDisplayColour!;
                    cardStatus.statusID = 2;
                    break;

                // 2 Green 3 yellow 1 orange
                case "2310":
                    cardStatus.statusColour = YellowStatus.StatusDisplayColour!;
                    cardStatus.statusID = 2;
                    break;
                // 2 Green 3 yellow 1 red
                case "2301":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;

                // 2 Green 2 yellow 2 orange
                case "2220":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 2 Green 2 yellow 1 orange 1 red
                case "2211":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 2 Green 2 yellow 2 red
                case "2202":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;

                // 2 Green 1 yellow 3 orange
                case "2130":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 2 Green 1 yellow 2 orange 1 red
                case "2121":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 2 Green 1 yellow 1 orange 2 red
                case "2112":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 2 Green 1 yellow 3 red
                case "2103":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;

                // 2 Green 4 red
                case "2040":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 2 Green 4 red
                case "2031":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 2 Green 2 orange 2 red
                case "2022":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 2 Green 1 orange 3 red
                case "2013":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 2 Green 4 red
                case "2004":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;

                // 1 Green 5 yellow
                case "1500":
                    cardStatus.statusColour = YellowStatus.StatusDisplayColour!;
                    cardStatus.statusID = 2;
                    break;

                // 1 Green 4 yellow 1 Orange
                case "1410":
                    cardStatus.statusColour = YellowStatus.StatusDisplayColour!;
                    cardStatus.statusID = 2;
                    break;
                // 1 Green 4 yellow 1 red
                case "1401":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;

                // 1 Green 3 yellow 2 orange
                case "1320":
                    cardStatus.statusColour = YellowStatus.StatusDisplayColour!;
                    cardStatus.statusID = 2;
                    break;
                // 1 Green 3 yellow 1 orange 1 red
                case "1311":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 1 Green 3 yellow 2 red
                case "1302":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;

                // 1 Green 2 yellow 3 orange
                case "1230":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 1 Green 2 yellow 2 orange 1 red
                case "1221":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 1 Green 2 yellow 1 orange 2 red
                case "1212":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 1 Green 2 yellow 3 red
                case "1203":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;

                // 1 Green 1 yellow 4 orange
                case "1140":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 1 Green 1 yellow 3 orange  1 red
                case "1131":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 1 Green 1 yellow 2 orange 2 red
                case "1122":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 1 Green 1 yellow 1 orange  3 red
                case "1113":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 1 Green 1 yellow 4 red
                case "1104":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;

                // 1 Green 5 orange
                case "1050":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 1 Green 4 orange 1 red
                case "1041":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 1 Green 3 orange 2 red
                case "1032":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 1 Green 2 orange 3 red
                case "1023":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 1 Green 1 orange 4 red
                case "1014":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 1 Green 5 red
                case "1005":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;

                // 6 yellow
                case "0600":
                    cardStatus.statusColour = YellowStatus.StatusDisplayColour!;
                    cardStatus.statusID = 2;
                    break;

                // 5 yellow 1 orange
                case "0510":
                    cardStatus.statusColour = YellowStatus.StatusDisplayColour!;
                    cardStatus.statusID = 2;
                    break;
                // 5 yellow 1 red
                case "0501":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;

                // 4 yellow 2 orange
                case "0420":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 4 yellow 2 red
                case "0402":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 4 yellow 1 orange 1 red
                case "0411":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;

                // 3 yellow 3 orange
                case "0330":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 3 yellow 2 orange 1 red
                case "0321":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 3 yellow 1 orange 2 red
                case "0312":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 3 yellow  3 red
                case "0303":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;

                // 2 yellow 3 orange 1 red
                case "0231":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 2 yellow 1 orange3 red
                case "0213":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 2 yellow 2 orange 2 red
                case "0222":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 2 yellow 4 orange
                case "0240":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 2 yellow 4 orange
                case "0204":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;

                // 1 yellow 5 orange
                case "0150":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 1 yellow 4 orange 1 red
                case "0141":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 1 yellow 3 orange 2 red
                case "0132":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 1 yellow 2 orange 3 red
                case "0123":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 1 yellow 1 orange 4 red
                case "0114":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 1 yellow 5 red
                case "0105":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;

                // 6 orange
                case "0060":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 5 orange 1 red
                case "0051":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 4 orange 2 red
                case "0042":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 3 orange  3 red
                case "0033":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 2 orange 4 red
                case "0024":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 1 orange 5 red
                case "0015":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 6 red
                case "0006":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
            }
            return cardStatus;
        }

        private CardStatus CalculateFiveStatuses(string selectedStatuses)
        {
            CardStatus cardStatus = new();

            switch (selectedStatuses)
            {
                // 5 green
                case "5000":
                    cardStatus.statusColour = GreenStatus.StatusDisplayColour!;
                    cardStatus.statusID = 1;
                    break;

                // 4 green 1 yellow
                case "4100":
                    cardStatus.statusColour = GreenStatus.StatusDisplayColour!;
                    cardStatus.statusID = 1;
                    break;

                // 4 green one orange
                case "4010":
                    cardStatus.statusColour = YellowStatus.StatusDisplayColour!;
                    cardStatus.statusID = 2;
                    break;
                // 4 green 1 red
                case "4001":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;

                // 3 green 2 yellow
                case "3200":
                    cardStatus.statusColour = YellowStatus.StatusDisplayColour!;
                    cardStatus.statusID = 2;
                    break;

                // 3 green 1 yellow 1 orange
                case "3110":
                    cardStatus.statusColour = YellowStatus.StatusDisplayColour!;
                    cardStatus.statusID = 2;
                    break;
                // 3 green 1 yellow 1 red
                case "3101":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;

                // 3 green 2 orange
                case "3020":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 3 green 1 orange 1 red
                case "3011":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 3 green 2 red
                case "3002":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;

                // 3 yellow 2 green
                case "2300":
                    cardStatus.statusColour = YellowStatus.StatusDisplayColour!;
                    cardStatus.statusID = 2;
                    break;

                // 2 green 2 yellow 1 orange
                case "2210":
                    cardStatus.statusColour = YellowStatus.StatusDisplayColour!;
                    cardStatus.statusID = 2;
                    break;
                // 2 green 2 yellow 1 red
                case "2201":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;

                // 2 green 1 yellow 2 orange
                case "2120":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 2 green 1 yellow 1 orange 1 red
                case "2111":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 2 green 1 yellow 2 red
                case "2102":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;

                // 3 orange 2 green
                case "2030":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 2 green 2 orange 1 red
                case "2021":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 2 green 1 orange 2 red
                case "2012":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 3 red 2 green
                case "2003":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;

                // 4 yellow 1 green
                case "1400":
                    cardStatus.statusColour = YellowStatus.StatusDisplayColour!;
                    cardStatus.statusID = 2;
                    break;

                // 1 green 3 yellow 1 orange
                case "1310":
                    cardStatus.statusColour = YellowStatus.StatusDisplayColour!;
                    cardStatus.statusID = 2;
                    break;
                // 1 green 3 yellow 1 red
                case "1301":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 2;
                    break;

                // 1 green 2yellow 2 orange
                case "1220":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 1 green 2 yellow 1 orange 1 red
                case "1211":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 1 green 2 yellow 2 red
                case "1202":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;

                // 1 green 1 yellow 3 orange
                case "1130":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 1 green 1 yellow 2 orange 1 red
                case "1121":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 1 green 1 yellow 1 orange 2 red
                case "1112":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 1 green 1 yellow 3 red
                case "1103":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;

                // 4 orange 1 green
                case "1040":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 1 green 3 orange 1 red
                case "1031":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 1 green 2 orange 2 red
                case "1022":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 1 green 1 orange 3 red
                case "1013":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 4 red 1 green
                case "1004":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;

                // 5 yellow
                case "0500":
                    cardStatus.statusColour = YellowStatus.StatusDisplayColour!;
                    cardStatus.statusID = 2;
                    break;

                // 4 yellow 1 orange
                case "0410":
                    cardStatus.statusColour = YellowStatus.StatusDisplayColour!;
                    cardStatus.statusID = 2;
                    break;
                // 4 yellow 1 red
                case "0401":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;

                // 3 yellow 2 orange
                case "0320":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 3 yellow 2 red
                case "0311":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 3 yellow 2 red
                case "0302":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;

                // 3 orange 2 green
                case "0230":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 2 yellow 2 orange 1 red
                case "0221":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 2 yellow 1 orange 2 red
                case "0212":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 3 red 2 yellow
                case "0203":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;

                // 1 yellow 4 orange
                case "0140":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                //  1 yellow 3 orange 1 red
                case "0131":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 1 yellow 2 orange 2 red
                case "0122":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 1 yellow 1 orange 3 red
                case "0113":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                //1 yellow 4 red
                case "0104":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;

                // 5 orange
                case "0050":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 4 orange 1 red
                case "0041":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 3 orange 2 red
                case "0032":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 3 red 2 orange
                case "0023":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 4 red 1 orange
                case "0014":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 5 red
                case "0005":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;

                default:
                    cardStatus.statusColour = White;
                    cardStatus.statusID = 5;
                    break;
            }

            return cardStatus;
        }

        private CardStatus CalculateFourStatuses(string selectedStatuses)
        {
            CardStatus cardStatus = new();

            switch (selectedStatuses)
            {
                // 4 green
                case "4000":
                    cardStatus.statusColour = GreenStatus.StatusDisplayColour!;
                    cardStatus.statusID = 1;
                    break;

                // 3 green 1 yellow
                case "3100":
                    cardStatus.statusColour = GreenStatus.StatusDisplayColour!;
                    cardStatus.statusID = 1;
                    break;

                // 3 green 1 orange
                case "3010":
                    cardStatus.statusColour = YellowStatus.StatusDisplayColour!;
                    cardStatus.statusID = 2;
                    break;
                // 3 green 1 red
                case "3001":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;

                // 2 green 2 yellow
                case "2200":
                    cardStatus.statusColour = YellowStatus.StatusDisplayColour!;
                    cardStatus.statusID = 2;
                    break;

                // 2 green 1 yellow 1 orange
                case "2110":
                    cardStatus.statusColour = YellowStatus.StatusDisplayColour!;
                    cardStatus.statusID = 2;
                    break;
                // 2 green 1 yellow 1 red
                case "2101":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;

                // 2 green 2 orange
                case "2020":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 2 green 1 orange 1 red
                case "2011":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 2 green 2 red
                case "2002":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;

                // 3 yellow 1 green
                case "1300":
                    cardStatus.statusColour = YellowStatus.StatusDisplayColour!;
                    cardStatus.statusID = 2;
                    break;

                // 2 yellow 1 green 1 orange
                case "1210":
                    cardStatus.statusColour = YellowStatus.StatusDisplayColour!;
                    cardStatus.statusID = 2;
                    break;
                // 2 yellow 1 green 1 red
                case "1201":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;

                // 1 green 1 yellow 2 orange
                case "1120":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 1 green 1 yellow 1 orange 1 red
                case "1111":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 1 green 1 yellow 2 red
                case "1102":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;

                // 3 orange 1 green
                case "1030":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 2 orange 1 green 1 red
                case "1021":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 1 green 1 orange 2 red
                case "1012":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 1 green  3 red
                case "1003":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;

                // 4 yellow
                case "0400":
                    cardStatus.statusColour = YellowStatus.StatusDisplayColour!;
                    cardStatus.statusID = 2;
                    break;

                // 3 yellow 1 orange
                case "0310":
                    cardStatus.statusColour = YellowStatus.StatusDisplayColour!;
                    cardStatus.statusID = 2;
                    break;
                // 3 yellow 1 red
                case "0301":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;

                // 2 yellow 2 orange
                case "0220":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 2 yellow 1 orange 1 red
                case "0211":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 2 yellow 2 red
                case "0202":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;

                // 3 orange 1 yellow
                case "0130":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 2 orange 1 yellow 1 red
                case "0121":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 1 orange 1 yellow 2 red
                case "0112":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 3 red 1 yellow
                case "0103":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;

                // 4 orange
                case "0040":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 3 orange 1 red
                case "0031":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 2 orange 2 red
                case "0022":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 3 red 1 orange
                case "0013":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 4 red
                case "0004":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;

                default:
                    cardStatus.statusColour = White;
                    cardStatus.statusID = 5;
                    break;
            }
            return cardStatus;
        }

        private CardStatus CalculateThreeStatuses(string selectedStatuses)
        {
            CardStatus cardStatus = new();

            switch (selectedStatuses)
            {
                // 3 green
                case "3000":
                    cardStatus.statusColour = GreenStatus.StatusDisplayColour!;
                    cardStatus.statusID = 1;
                    break;

                // 2 green 1 yellow
                case "2100":
                    cardStatus.statusColour = YellowStatus.StatusDisplayColour!;
                    cardStatus.statusID = 2;
                    break;

                // 2 green 1 orange
                case "2010":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 2 green 1 red
                case "2001":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;

                // 2 yellow 1 green
                case "1200":
                    cardStatus.statusColour = YellowStatus.StatusDisplayColour!;
                    cardStatus.statusID = 2;
                    break;

                //1 green one yellow 1 orange
                case "1110":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                //1 green one yellow 1 red
                case "1101":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;

                // 2 orange 1 green
                case "1020":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 1 green 1 orange one red
                case "1011":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 2 red 1 green
                case "1002":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;

                // 3 yellow
                case "0300":
                    cardStatus.statusColour = YellowStatus.StatusDisplayColour!;
                    cardStatus.statusID = 2;
                    break;

                // 2 yellow 1 orange
                case "0210":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 2 yellow 1 red
                case "0201":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;

                // 2 orange 1 yellow
                case "0120":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 1 yellow 1 orange 1 red
                case "0111":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 1 yellow 1 orange 1 red
                case "0102":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;

                // 3 orange
                case "0030":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 2 orange 1 red
                case "0021":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 2 red 1 orange
                case "0012":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 3 red
                case "0003":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;

                default:
                    cardStatus.statusColour = White;
                    cardStatus.statusID = 5;
                    break;
            }
            return cardStatus;
        }

        private CardStatus CalculateTwoStatuses(string selectedStatuses)
        {
            CardStatus cardStatus = new();

            switch (selectedStatuses)
            {
                // 2 green
                case "2000":
                    cardStatus.statusColour = GreenStatus.StatusDisplayColour!;
                    cardStatus.statusID = 1;
                    break;
                // 2 green 1 yellow
                case "1100":
                    cardStatus.statusColour = YellowStatus.StatusDisplayColour!;
                    cardStatus.statusID = 2;
                    break;
                // 1 green 1 orange
                case "1010":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 1 green 1 red
                case "1001":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 2 yellow
                case "0200":
                    cardStatus.statusColour = YellowStatus.StatusDisplayColour!;
                    cardStatus.statusID = 2;
                    break;
                // 1 yellow 1 orange
                case "0110":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 1 yellow 1 red
                case "0101":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 2 orange
                case "0020":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                // 1 orange 1 red
                case "0011":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;
                // 2 red
                case "0002":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;

                default:
                    cardStatus.statusColour = White;
                    cardStatus.statusID = 5;
                    break;
            }
            return cardStatus;
        }

        private CardStatus CalculateOneStatuses(string selectedStatuses)
        {
            CardStatus cardStatus = new();

            switch (selectedStatuses)
            {
                // 1 green
                case "1000":
                    cardStatus.statusColour = GreenStatus.StatusDisplayColour!;
                    cardStatus.statusID = 1;
                    break;
                // yellow
                case "0100":
                    cardStatus.statusColour = YellowStatus.StatusDisplayColour!;
                    cardStatus.statusID = 2;
                    break;
                //  1 orange
                case "0010":
                    cardStatus.statusColour = OrangeStatus.StatusDisplayColour!;
                    cardStatus.statusID = 3;
                    break;
                //  1 red
                case "0001":
                    cardStatus.statusColour = RedStatus.StatusDisplayColour!;
                    cardStatus.statusID = 4;
                    break;

                default:
                    cardStatus.statusColour = White;
                    cardStatus.statusID = 5;
                    break;
            }
            return cardStatus;
        }
    }
}