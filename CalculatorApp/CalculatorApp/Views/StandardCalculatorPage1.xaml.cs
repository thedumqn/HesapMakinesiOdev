using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace CalculatorApp.Views
{
    public partial class StandardCalculatorPage : ContentPage
    {
        private string currentNumber = "";
        private string currentCalculation = "";
        private bool isOperationPending = false;
        private string pendingOperator = "";
        private double firstOperand = 0;
        private bool isNewCalculation = true;
        private bool isLastButtonEquals = false;

        public StandardCalculatorPage()
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

        // Sadece mevcut girişi temizle
        private void ClearEntryClicked(object sender, EventArgs e)
        {
            currentNumber = "0";
            ResultEntry.Text = currentNumber;
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

        // Yüzde butonu için olay işleyici
        private void PercentClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(currentNumber))
            {
                double number;
                if (double.TryParse(currentNumber, NumberStyles.Any, CultureInfo.CurrentCulture, out number))
                {
                    if (isOperationPending)
                    {
                        // İlk operandın yüzdesi olarak hesapla
                        number = firstOperand * (number / 100);
                    }
                    else
                    {
                        number /= 100;
                    }
                    currentNumber = number.ToString(CultureInfo.CurrentCulture);
                    ResultEntry.Text = currentNumber;
                }
            }
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

        // 1/x butonu için olay işleyici
        private void ReciprocalClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(currentNumber))
            {
                double number;
                if (double.TryParse(currentNumber, NumberStyles.Any, CultureInfo.CurrentCulture, out number))
                {
                    if (number == 0)
                    {
                        DisplayAlert("Hata", "Sıfıra bölme hatası!", "Tamam");
                        return;
                    }
                    number = 1 / number;
                    currentNumber = number.ToString(CultureInfo.CurrentCulture);
                    ResultEntry.Text = currentNumber;
                }
            }
        }
    }
}