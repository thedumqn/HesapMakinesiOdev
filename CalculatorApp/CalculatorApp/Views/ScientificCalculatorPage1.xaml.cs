using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace CalculatorApp.Views
{
    public partial class ScientificCalculatorPage : ContentPage
    {
        private string currentNumber = "";
        private string currentCalculation = "";
        private bool isOperationPending = false;
        private string pendingOperator = "";
        private double firstOperand = 0;
        private bool isNewCalculation = true;
        private bool isLastButtonEquals = false;
        private bool isRadianMode = true; // true = radyan, false = derece

        public ScientificCalculatorPage()
        {
            InitializeComponent();
        }

        // Sayı butonları için olay işleyici
        private void NumberClicked(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            string digit = button.Text;

            if (isNewCalculation || isLastButtonEquals)
            {
                currentNumber = digit;
                isNewCalculation = false;
                isLastButtonEquals = false;
            }
            else if (currentNumber == "0")
            {
                currentNumber = digit;
            }
            else
            {
                currentNumber += digit;
            }

            ResultEntry.Text = currentNumber;
        }

        // Ondalık nokta için olay işleyici
        private void DecimalClicked(object sender, EventArgs e)
        {
            if (isLastButtonEquals)
            {
                currentNumber = "0,";
                isLastButtonEquals = false;
                isNewCalculation = false;
            }
            else if (!currentNumber.Contains(","))
            {
                if (string.IsNullOrEmpty(currentNumber))
                    currentNumber = "0,";
                else
                    currentNumber += ",";
            }

            ResultEntry.Text = currentNumber;
        }

        // İşlem operatörleri için olay işleyici
        private void OperatorClicked(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            string operatorSymbol = button.Text;

            if (!string.IsNullOrEmpty(currentNumber) || isLastButtonEquals)
            {
                double number;
                if (double.TryParse(currentNumber, NumberStyles.Any, CultureInfo.CurrentCulture, out number))
                {
                    if (isOperationPending)
                    {
                        PerformCalculation();
                    }
                    else
                    {
                        firstOperand = number;
                    }

                    pendingOperator = operatorSymbol;
                    isOperationPending = true;
                    currentCalculation = $"{firstOperand} {pendingOperator}";
                    PreviousCalculationLabel.Text = currentCalculation;
                    isNewCalculation = true;
                    isLastButtonEquals = false;
                }
            }
            else if (isOperationPending)
            {
                // Operatörü değiştir
                pendingOperator = operatorSymbol;
                currentCalculation = $"{firstOperand} {pendingOperator}";
                PreviousCalculationLabel.Text = currentCalculation;
            }
        }

        // Eşittir butonu için olay işleyici
        private void EqualsClicked(object sender, EventArgs e)
        {
            if (isOperationPending && !string.IsNullOrEmpty(currentNumber))
            {
                PerformCalculation();
                isOperationPending = false;
                pendingOperator = "";
                currentCalculation = "";
                PreviousCalculationLabel.Text = "";
                isLastButtonEquals = true;
            }
        }

        // Gerçek hesaplama işlemini gerçekleştir
        private void PerformCalculation()
        {
            double secondOperand;
            if (double.TryParse(currentNumber, NumberStyles.Any, CultureInfo.CurrentCulture, out secondOperand))
            {
                try
                {
                    switch (pendingOperator)
                    {
                        case "+":
                            firstOperand += secondOperand;
                            break;
                        case "-":
                            firstOperand -= secondOperand;
                            break;
                        case "×":
                            firstOperand *= secondOperand;
                            break;
                        case "÷":
                            if (secondOperand == 0)
                            {
                                throw new DivideByZeroException("Sıfıra bölme hatası!");
                            }
                            firstOperand /= secondOperand;
                            break;
                        case "Mod":
                            firstOperand %= secondOperand;
                            break;
                        case "x^y":
                            firstOperand = Math.Pow(firstOperand, secondOperand);
                            break;
                    }

                    currentNumber = firstOperand.ToString(CultureInfo.CurrentCulture);
                    ResultEntry.Text = currentNumber;
                }
                catch (DivideByZeroException ex)
                {
                    DisplayAlert("Hata", ex.Message, "Tamam");
                    ClearAll();
                }
                catch (Exception ex)
                {
                    DisplayAlert("Hata", "Hesaplama sırasında bir hata oluştu: " + ex.Message, "Tamam");
                    ClearAll();
                }
            }
        }

        // Temizle butonu için olay işleyici
        private void ClearClicked(object sender, EventArgs e)
        {
            ClearAll();
        }

        // Tüm hesaplamayı temizle
        private void ClearAll()
        {
            currentNumber = "0";
            currentCalculation = "";
            isOperationPending = false;
            pendingOperator = "";
            firstOperand = 0;
            isNewCalculation = true;
            isLastButtonEquals = false;
            ResultEntry.Text = currentNumber;
            PreviousCalculationLabel.Text = "";
        }

        // Backspace butonu için olay işleyici
        private void BackspaceClicked(object sender, EventArgs e)
        {
            if (currentNumber.Length > 1)
            {
                currentNumber = currentNumber.Substring(0, currentNumber.Length - 1);
            }
            else
            {
                currentNumber = "0";
            }
            ResultEntry.Text = currentNumber;
        }

        // İşaret değiştirme butonu için olay işleyici
        private void NegateClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(currentNumber) && currentNumber != "0")
            {
                if (currentNumber.StartsWith("-"))
                {
                    currentNumber = currentNumber.Substring(1);
                }
                else
                {
                    currentNumber = "-" + currentNumber;
                }
                ResultEntry.Text = currentNumber;
            }
        }

        // Kare alma butonu için olay işleyici
        private void SquareClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(currentNumber))
            {
                double number;
                if (double.TryParse(currentNumber, NumberStyles.Any, CultureInfo.CurrentCulture, out number))
                {
                    number *= number;
                    currentNumber = number.ToString(CultureInfo.CurrentCulture);
                    ResultEntry.Text = currentNumber;
                }
            }
        }

        // Karekök butonu için olay işleyici
        private void SqrtClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(currentNumber))
            {
                double number;
                if (double.TryParse(currentNumber, NumberStyles.Any, CultureInfo.CurrentCulture, out number))
                {
                    if (number < 0)
                    {
                        DisplayAlert("Hata", "Negatif sayının karekökü alınamaz!", "Tamam");
                        return;
                    }
                    number = Math.Sqrt(number);
                    currentNumber = number.ToString(CultureInfo.CurrentCulture);
                    ResultEntry.Text = currentNumber;
                }
            }
        }

        // Derece/Radyan mod değiştirme butonu için olay işleyici
        private void DegreeClicked(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            isRadianMode = !isRadianMode;
            button.Text = isRadianMode ? "RAD" : "DEG";
        }

        // Pi sayısı butonu için olay işleyici
        private void PiClicked(object sender, EventArgs e)
        {
            currentNumber = Math.PI.ToString(CultureInfo.CurrentCulture);
            ResultEntry.Text = currentNumber;
            isNewCalculation = false;
            isLastButtonEquals = false;
        }

        // e sayısı butonu için olay işleyici
        private void EClicked(object sender, EventArgs e)
        {
            currentNumber = Math.E.ToString(CultureInfo.CurrentCulture);
            ResultEntry.Text = currentNumber;
            isNewCalculation = false;
            isLastButtonEquals = false;
        }

        // Sinüs butonu için olay işleyici
        private void SinClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(currentNumber))
            {
                double number;
                if (double.TryParse(currentNumber, NumberStyles.Any, CultureInfo.CurrentCulture, out number))
                {
                    if (!isRadianMode)
                    {
                        // Derece ise radyana çevir
                        number = number * Math.PI / 180;
                    }
                    number = Math.Sin(number);
                    currentNumber = number.ToString(CultureInfo.CurrentCulture);
                    ResultEntry.Text = currentNumber;
                }
            }
        }

        // Kosinüs butonu için olay işleyici
        private void CosClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(currentNumber))
            {
                double number;
                if (double.TryParse(currentNumber, NumberStyles.Any, CultureInfo.CurrentCulture, out number))
                {
                    if (!isRadianMode)
                    {
                        // Derece ise radyana çevir
                        number = number * Math.PI / 180;
                    }
                    number = Math.Cos(number);
                    currentNumber = number.ToString(CultureInfo.CurrentCulture);
                    ResultEntry.Text = currentNumber;
                }
            }
        }

        // Tanjant butonu için olay işleyici
        private void TanClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(currentNumber))
            {
                double number;
                if (double.TryParse(currentNumber, NumberStyles.Any, CultureInfo.CurrentCulture, out number))
                {
                    if (!isRadianMode)
                    {
                        // Derece ise radyana çevir
                        number = number * Math.PI / 180;
                    }

                    // 90 derece ve katları için kontrol
                    double modValue = number % Math.PI;
                    if (Math.Abs(modValue - Math.PI / 2) < 0.0000001)
                    {
                        DisplayAlert("Hata", "Tanjant fonksiyonu bu değer için tanımsızdır!", "Tamam");
                        return;
                    }

                    number = Math.Tan(number);
                    currentNumber = number.ToString(CultureInfo.CurrentCulture);
                    ResultEntry.Text = currentNumber;
                }
            }
        }

        // Logaritma butonu için olay işleyici
        private void LogClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(currentNumber))
            {
                double number;
                if (double.TryParse(currentNumber, NumberStyles.Any, CultureInfo.CurrentCulture, out number))
                {
                    if (number <= 0)
                    {
                        DisplayAlert("Hata", "Logaritması alınacak sayı pozitif olmalıdır!", "Tamam");
                        return;
                    }
                    number = Math.Log10(number);
                    currentNumber = number.ToString(CultureInfo.CurrentCulture);
                    ResultEntry.Text = currentNumber;
                }
            }
        }

        // Üs alma butonu için olay işleyici
        private void PowerClicked(object sender, EventArgs e)
        {
            OperatorClicked(sender, e);
        }

        // 10 üzeri x butonu için olay işleyici
        private void TenPowXClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(currentNumber))
            {
                double number;
                if (double.TryParse(currentNumber, NumberStyles.Any, CultureInfo.CurrentCulture, out number))
                {
                    number = Math.Pow(10, number);
                    currentNumber = number.ToString(CultureInfo.CurrentCulture);
                    ResultEntry.Text = currentNumber;
                }
            }
        }

        // Faktöriyel butonu için olay işleyici
        private void FactorialClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(currentNumber))
            {
                double number;
                if (double.TryParse(currentNumber, NumberStyles.Any, CultureInfo.CurrentCulture, out number))
                {
                    if (number < 0 || number != Math.Floor(number))
                    {
                        DisplayAlert("Hata", "Faktöriyel sadece pozitif tam sayılar için hesaplanabilir!", "Tamam");
                        return;
                    }

                    if (number > 170)
                    {
                        DisplayAlert("Hata", "Faktöriyel taşması! Hesaplanamayacak kadar büyük bir sayı.", "Tamam");
                        return;
                    }

                    double factorial = 1;
                    for (int i = 2; i <= number; i++)
                    {
                        factorial *= i;
                    }

                    currentNumber = factorial.ToString(CultureInfo.CurrentCulture);
                    ResultEntry.Text = currentNumber;
                }
            }
        }

        // Exp butonu için olay işleyici
        private void ExpClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(currentNumber))
            {
                double number;
                if (double.TryParse(currentNumber, NumberStyles.Any, CultureInfo.CurrentCulture, out number))
                {
                    number = Math.Exp(number);
                    currentNumber = number.ToString(CultureInfo.CurrentCulture);
                    ResultEntry.Text = currentNumber;
                }
            }
        }

        // Mod butonu için olay işleyici
        private void ModClicked(object sender, EventArgs e)
        {
            OperatorClicked(sender, e);
        }

        // Parantez butonları için olay işleyici
        private void ParenthesisClicked(object sender, EventArgs e)
        {
            // Kompleks ifadelerin hesaplanması için ileri seviye hesap makinesi özelliği eklenmeli
            // Bu ödevin kapsamı dışında olduğu için burada sadece uyarı verelim
            DisplayAlert("Bilgi", "Parantez içeren karmaşık ifadelerin hesaplanması bu ödevin kapsamı dışındadır.", "Tamam");
        }
    }
}