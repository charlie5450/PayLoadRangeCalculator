using LiveCharts.Wpf;
using LiveCharts;
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
            try
            {
                double oew = ParseInput(OewTextBox.Text);
                double payload = ParseInput(PayloadTextBox.Text);
                double fuelWeight = ParseInput(FuelWeightTextBox.Text);
                double velocity = ParseInput(VelocityTextBox.Text);
                double ldRatio = ParseInput(LDTextBox.Text);
                double sfc = ParseInput(SFCTextBox.Text);

                // 단위 변환
                if (((ComboBoxItem)OewUnitComboBox.SelectedItem)?.Content.ToString() == "lb")
                    oew *= 0.453592;
                if (((ComboBoxItem)PayloadUnitComboBox.SelectedItem)?.Content.ToString() == "lb")
                    payload *= 0.453592;
                if (((ComboBoxItem)FuelWeightUnitComboBox.SelectedItem)?.Content.ToString() == "lb")
                    fuelWeight *= 0.453592;
                if (((ComboBoxItem)VelocityUnitComboBox.SelectedItem)?.Content.ToString() == "m/s")
                    velocity *= 3.6;
                if (((ComboBoxItem)SFCUnitComboBox.SelectedItem)?.Content.ToString() == "1/s")
                    sfc *= 3600;

                // 자동 계산된 초기 중량
                double initialWeight = oew + payload + fuelWeight;
                double finalWeight = initialWeight - fuelWeight;

                if (finalWeight <= 0) throw new Exception("연료 중량이 초기 중량보다 많습니다.");

                double range = (velocity / sfc) * ldRatio * Math.Log(initialWeight / finalWeight);

                ResultTextBlock.Text = $"예상 항속거리: {range:N2} km";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"오류: {ex.Message}", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void FormatNumberWithComma(object sender, TextChangedEventArgs e)
        {
            var textbox = sender as TextBox;
            if (textbox == null) return;

            string input = textbox.Text;

            // 입력 중간 상태 예외 처리 (".", ".0", "0.", "0.0" 등)
            if (string.IsNullOrEmpty(input) || input.EndsWith(".") || Regex.IsMatch(input, @"\.\d{0,2}$"))
                return;

            string raw = input.Replace(",", "");

            if (double.TryParse(raw, out double value))
            {
                string formatted = value.ToString("#,##0.##"); // 천 단위 콤마 + 소수점 2자리까지
                if (formatted != input)
                {
                    int caret = textbox.SelectionStart;
                    textbox.Text = formatted;
                    textbox.SelectionStart = Math.Min(caret + 1, textbox.Text.Length);
                }
            }
        }

        private double ParseInput(string text)
        {
            string clean = text.Replace(",", "");
            return double.Parse(clean);
        }
    }
}
