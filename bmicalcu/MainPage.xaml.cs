
//fuckass project

using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace bmicalcu;

public partial class MainPage : ContentPage
{
    private int _age;
    private double _weightKg;
    private double _heightCm = 170;
    private bool _isFemale;
    private bool _weightInLbs;
    private bool _heightInFeet;

    // ft/in inputs
    private int _feet = 5;
    private int _inches = 7;

    public MainPage()
    {
        InitializeComponent();

        // ensure pickers have a default value on startup
        WeightUnitPicker.SelectedIndex = 0;
        HeightUnitPicker.SelectedIndex = 0;
        HeightLabel.Text = "170 cm";
    }

    private void OnAgeTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (int.TryParse(e.NewTextValue, out int age))
            _age = age;
    }

    private void OnWeightTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (double.TryParse(e.NewTextValue, out double value))
            _weightKg = _weightInLbs ? value * 0.453592 : value;
    }

    private void OnHeightChanged(object? sender, ValueChangedEventArgs e)
    {
        _heightCm = e.NewValue;
        HeightLabel.Text = $"{_heightCm:F0} cm";
    }

    // --- ft / in handlers ---

    private void OnFeetTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (int.TryParse(e.NewTextValue, out int feet))
        {
            _feet = Math.Clamp(feet, 3, 7);
            UpdateHeightFromFtIn();
        }
    }

    private void OnInchesTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (int.TryParse(e.NewTextValue, out int inches))
        {
            _inches = Math.Clamp(inches, 0, 11);
            UpdateHeightFromFtIn();
        }
    }

    private void UpdateHeightFromFtIn()
    {
        double totalInches = (_feet * 12) + _inches;
        _heightCm = totalInches * 2.54;
    }

    // --- gender ---

    private void OnGenderToggle(object? sender, EventArgs e)
    {
        _isFemale = !_isFemale;
        if (_isFemale)
        {
            GenderToggleButton.Text = "♀ Female";
            GenderToggleButton.BackgroundColor = Color.FromArgb("#2A001A");
            GenderToggleButton.TextColor = Color.FromArgb("#FF85C2");
        }
        else
        {
            GenderToggleButton.Text = "♂ Male";
            GenderToggleButton.BackgroundColor = Color.FromArgb("#1E2A00");
            GenderToggleButton.TextColor = Color.FromArgb("#E8FF47");
        }
    }

    // --- unit pickers ---

    private void OnWeightUnitChanged(object? sender, EventArgs e)
    {
        _weightInLbs = WeightUnitPicker.SelectedIndex == 1;
        WeightUnitLabel.Text = "WEIGHT";
        WeightSubLabel.Text = _weightInLbs ? "lb" : "kg";

        // clear entry so user re-enters in the new unit
        WeightEntry.Text = string.Empty;
        _weightKg = 0;
    }

    private void OnHeightUnitChanged(object? sender, EventArgs e)
    {
        _heightInFeet = HeightUnitPicker.SelectedIndex == 1;

        if (_heightInFeet)
        {
            CmHeightPanel.IsVisible = false;
            FtInHeightPanel.IsVisible = true;
            HeightUnitLabel.Text = "HEIGHT";

            // default to 5'7 and compute ay wow tall king
            _feet = 5;
            _inches = 7;
            FeetEntry.Text = "5";
            InchesEntry.Text = "7";
            UpdateHeightFromFtIn();
        }
        else
        {
            CmHeightPanel.IsVisible = true;
            FtInHeightPanel.IsVisible = false;
            HeightUnitLabel.Text = "HEIGHT";
            _heightCm = HeightSlider.Value;
            HeightLabel.Text = $"{_heightCm:F0} cm";
        }
    }

    // --- calculate ---

    private async void OnCalculateClicked(object? sender, EventArgs e)
    {
        if (_age <= 0)
        {
            await this.DisplayAlertAsync("Missing Info", "Please enter your age.", "OK");
            return;
        }
        if (_weightKg <= 0)
        {
            await this.DisplayAlertAsync("Missing Info", "Please enter your weight.", "OK");
            return;
        }
        if (_heightCm <= 0)
        {
            await this.DisplayAlertAsync("Missing Info", "Height value is invalid.", "OK");
            return;
        }

        double heightMeters = _heightCm / 100.0;
        double bmi = _weightKg / (heightMeters * heightMeters);

        BmiWhole.Text = bmi.ToString("F1");

        if (bmi < 18.5)
        {
            CategoryLabel.Text = "UNDERWEIGHT";
            CategoryLabel.TextColor = Color.FromArgb("#5BA4FF");
            CategoryDesc.Text = "Consider speaking with a nutritionist.";
        }
        else if (bmi < 25)
        {
            CategoryLabel.Text = "NORMAL";
            CategoryLabel.TextColor = Color.FromArgb("#E8FF47");
            CategoryDesc.Text = "You're within a healthy BMI range.";
        }
        else if (bmi < 30)
        {
            CategoryLabel.Text = "OVERWEIGHT";
            CategoryLabel.TextColor = Color.FromArgb("#FFB347");
            CategoryDesc.Text = "A balanced diet and exercise may help.";
        }
        else
        {
            CategoryLabel.Text = "OBESE";
            CategoryLabel.TextColor = Color.FromArgb("#FF5C5C");
            CategoryDesc.Text = "Please consult a healthcare professional.";
        }

        InputPage.IsVisible = false;
        ResultsPage.IsVisible = true;
    }

    private void OnRecalculateClicked(object? sender, EventArgs e)
    {
        ResultsPage.IsVisible = false;
        InputPage.IsVisible = true;
    }
}