using UnityEngine;
using System.Numerics;
namespace Util
{
    public class CommonUtil
    {
        static readonly string[] CurrencyUnits = new string[] { "", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY", "AZ", "BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", "BM", "BN", "BO", "BP", "BQ", "BR", "BS", "BT", "BU", "BV", "BW", "BX", "BY", "BZ", "CA", "CB", "CC", "CD", "CE", "CF", "CG", "CH", "CI", "CJ", "CK", "CL", "CM", "CN", "CO", "CP", "CQ", "CR", "CS", "CT", "CU", "CV", "CW", "CX", };

        public const int STAT_PERCENT = 100;

        public const int FIRST_STAGE = 10001;
        public static readonly string[] UnitGradeColor = new string[] { "#DFE1D1", "#37C3E7", "#37BB56", "#BF3EC5", "#DE8134" };

        public static readonly int[] TestUnit = new int[] { 10001, 10005, 10009, 10010, 10014, 10016 };

        public static Color ColorUnitGrade(UnitGrade grade)
        {
            
            if ((int)grade - 1 > UnitGradeColor.Length)
            {
                LogManager.Instance.PrintLog(LogManager.enLogType.Error, "Grade Error!");
                return Color.clear;
            }

            return ColorHexToRGBA(UnitGradeColor[(int)grade - 1]);
        }
        public static Color ColorHexToRGBA(string hexColor, string alpha = "FF")
        {
            if (hexColor[0] != '#')
                hexColor = "#" + hexColor;
            if (hexColor.Length == 7)
                hexColor = hexColor + alpha;

            ColorUtility.TryParseHtmlString(hexColor, out Color c);
            return c;
        }

        /// <summary>
        /// BigInteger를 String으로 변환
        /// </summary>
        /// <param name="value"></param>
        /// <returns>ex) 10000 -> 10a</returns>
        public static string ConvertBigIntToString(BigInteger number)
        {
            string zero = "0";

            if (-1 < number && number < 1)
            {
                return zero;
            }

            //  부호 출력 문자열
            string significant = (number < 0) ? "-" : string.Empty;

            //  보여줄 숫자
            string showNumber = string.Empty;

            //  단위 문자열
            string unityString = string.Empty;

            //  패턴을 단순화 시키기 위해 무조건 지수 표현식으로 변경한 후 처리
            string[] partsSplit = number.ToString("E").Split('+');

            //  예외
            if (partsSplit.Length < 2)
            {
                return zero;
            }

            //  지수 (자릿수 표현)
            if (!int.TryParse(partsSplit[1], out int exponent))
            {
                Debug.LogWarningFormat("Failed - ToCurrentString({0}) : partSplit[1] = {1}", number, partsSplit[1]);
                return zero;
            }

            //  몫은 문자열 인덱스
            int quotient = exponent / 3;

            //  나머지는 정수부 자릿수 계산에 사용(10의 거듭제곱을 사용)
            int remainder = exponent % 3;

            //  1A 미만은 그냥 표현
            if (exponent < 3)
            {
                showNumber = number.ToString();
            }
            else
            {
                //  10의 거듭제곱을 구해서 자릿수 표현값을 만들어 준다.
                var temp = double.Parse(partsSplit[0].Replace("E", "")) * System.Math.Pow(10, remainder);

                //  소수 둘째자리까지만 출력한다.
                showNumber = temp.ToString("0.00");
            }

            unityString = CurrencyUnits[quotient];

            return string.Format("{0}{1}{2}", significant, showNumber, unityString);
        }

        /// <summary>
        /// 확률 계산
        /// </summary>
        /// <returns> 성공 여부 </returns>
        public static bool GetPercentageChance(float value)
        {
            int randomValue = Random.Range(0, 101);

            if (randomValue <= value)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}