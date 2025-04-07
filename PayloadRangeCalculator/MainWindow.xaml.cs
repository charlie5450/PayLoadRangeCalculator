using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PayloadRangeCalculator
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CalculateRange_Click(object sender, RoutedEventArgs e)
        {
            double oew = ToKg(ParseText(OEWTextBox), GetUnit(OEWUnitComboBox));
            double payload = ToKg(ParseText(PayloadTextBox), GetUnit(PayloadUnitComboBox));
            double fuel = ToKg(ParseText(FuelTextBox), GetUnit(FuelUnitComboBox));
            double velocity = ToKmPerHr(ParseText(VelocityTextBox), GetUnit(VelocityUnitComboBox));
            double sfc = ToKgPerHr(ParseText(SFCTextBox), GetUnit(SFCUnitComboBox));
            double ld = ParseText(LDTextBox);

            double totalWeight = oew + payload;
            double range_km = (velocity / sfc) * ld * Math.Log(totalWeight / (totalWeight - fuel));

            double final = FromKm(range_km, GetUnit(RangeUnitComboBox));
            RangeOutputText.Text = $"{final:N2}";
        }

        private double ParseText(TextBox tb)
        {
            return double.TryParse(tb.Text.Replace(",", ""), out double v) ? v : 0;
        }
        private string GetUnit(ComboBox cb)
        {
            return ((ComboBoxItem)cb.SelectedItem).Content.ToString();
        }


        private string AddComma(string input)
        {
            if (double.TryParse(input.Replace(",", ""), out double number))
            {
                return string.Format("{0:N0}", number); // 소수점 없이 콤마 포함
            }
            return input;
        }

        private void NumericTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var tb = sender as System.Windows.Controls.TextBox;
            if (tb == null) return;

            string raw = tb.Text.Replace(",", "");

            // 소수점 한 개만 허용하고, 나머지는 제거
            int dotCount = raw.Count(c => c == '.');
            if (dotCount > 1)
            {
                // 두 개 이상 소수점 찍은 경우 제거
                int firstDot = raw.IndexOf('.');
                raw = raw.Substring(0, firstDot + 1) + raw.Substring(firstDot + 1).Replace(".", "");
            }

            if (!double.TryParse(raw, out double number)) return;

            // 커서 위치 저장
            int caret = tb.CaretIndex;
            int oldLength = tb.Text.Length;

            // 소수점 입력 중이라면 포맷하지 않음 (맨 끝이 "." 또는 ".0"일 때 등)
            if (raw.EndsWith(".") || Regex.IsMatch(raw, @"\.\d{0,2}$"))
            {
                return;
            }

            // 소수점 둘째 자리까지 포맷
            string formatted = string.Format("{0:N0}", number);

            if (tb.Text != formatted)
            {
                tb.Text = formatted;
                int diff = tb.Text.Length - oldLength;
                tb.CaretIndex = Math.Max(0, caret + diff);
            }
        }

        private double ToKg(double value, string unit) => unit == "lb" ? value * 0.453592 : value;
        private double ToKmPerHr(double value, string unit)
        {
            switch (unit)
            {
                case "knots":
                    return value * 1.852;
                case "mph":
                    return value * 1.60934;
                default:
                    return value;
            }
        }
        private double ToKgPerHr(double value, string unit) => unit == "lb/hr" ? value * 0.453592 : value;
        private double FromKm(double value, string targetUnit)
        {
            switch (targetUnit)
            {
                case "NM":
                    return value / 1.852;
                case "mi":
                    return value / 1.60934;
                default:
                    return value;
            }
        }


    }
}
