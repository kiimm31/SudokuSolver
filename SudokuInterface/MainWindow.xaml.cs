using SudokuLogic;
using SudokuLogic.Constraints.Interface;
using SudokuLogic.Factory;
using SudokuLogic.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SudokuInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Board _board { get; set; }

        private static int _normalThickness => 1;
        private static int _boldThickness => 5;

        public MainWindow()
        {
            InitializeComponent();

            IEnumerable<IStrategy> strategies = StrategyFactory.CreateAllStrategies();

            IEnumerable<IConstraint> constrains = ConstraintFactory.CreateNormalSudokuContraints(strategies);

            _board = new Board(constrains);

            loadUi();
        }

        public void loadUi()
        {
            foreach (Cell cell in _board.Cells)
            {
                TextBox txtCell = new TextBox()
                {
                    Name = $"cellr{cell.Row}c{cell.Column}",
                    TextAlignment = TextAlignment.Center,
                    AcceptsReturn = false,
                    AcceptsTab = false,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Tag = cell,
                    BorderThickness = new Thickness(_normalThickness,
                                                    _normalThickness,
                                                    cell.Column % 3 == 0 ? _boldThickness : _normalThickness,
                                                    cell.Row % 3 == 0 ? _boldThickness : _normalThickness),
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    TextWrapping = TextWrapping.Wrap,
                    FontStretch = FontStretches.UltraExpanded
                };

                txtCell.TextChanged += TxtCell_TextChanged;
                txtCell.KeyDown += TxtCell_KeyDown;

                Grid.SetColumn(txtCell, cell.Column - 1);
                Grid.SetRow(txtCell, cell.Row - 1);

                gridBoard.Children.Add(txtCell);
            }
        }

        private void TxtCell_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.NumPad1:
                case Key.NumPad2:
                case Key.NumPad3:
                case Key.NumPad4:
                case Key.NumPad5:
                case Key.NumPad6:
                case Key.NumPad7:
                case Key.NumPad8:
                case Key.NumPad9:
                case Key.D1:
                case Key.D2:
                case Key.D3:
                case Key.D4:
                case Key.D5:
                case Key.D6:
                case Key.D7:
                case Key.D8:
                case Key.D9:
                    break;
                default:
                    e.Handled = true;
                    break;
            }
        }

        private void TxtCell_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox currentTxtBox = (TextBox)sender;

            int currentValue = 0;

            if (int.TryParse(currentTxtBox.Text, out currentValue))
            {
                Cell currentCell = (Cell)currentTxtBox.Tag;
                currentCell.SetValue(currentValue);
            }
        }

        private void btnSolveSudoku_Click(object sender, RoutedEventArgs e)
        {
            _board.AttemptSolve();

            reloadUi();
        }

        private void btnNextStep_Click(object sender, RoutedEventArgs e)
        {
            _board.StepIteration();

            reloadUi();
        }

        private void reloadUi()
        {
            foreach (Cell cell in _board.Cells.Where(x => x.Value > 0))
            {
                TextBox txtCell = (TextBox)gridBoard.Children.Cast<UIElement>().FirstOrDefault(x => Grid.GetRow(x) == cell.Row - 1 && Grid.GetColumn(x) == cell.Column - 1);

                if (txtCell != null && string.IsNullOrWhiteSpace(txtCell.Text))
                {
                    txtCell.Text = cell.Value.ToString();
                }
            }
        }

        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            if (_board.Check())
            {
                MessageBox.Show("Looks good to me!");
            }
            else
            {
                MessageBox.Show("Something looks off...");
            }
        }
    }
}
