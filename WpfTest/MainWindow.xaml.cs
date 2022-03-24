using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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

namespace WpfTest
{
    public partial class MainWindow : Window
    {
        Ellipse sleepBal;
        //int positionX = 590;
        //int positionY = 80;

        public MainWindow()
        {
            InitializeComponent();

            foreach (PropertyInfo info in typeof(Colors).GetProperties())
            {
                BrushConverter bc = new BrushConverter();
                SolidColorBrush deKleur = (SolidColorBrush)bc.ConvertFromString(info.Name);
                Kleur kleurke = new Kleur();
                kleurke.Borstel = deKleur;
                kleurke.Naam = info.Name;
                kleurke.Hex = deKleur.ToString();
                kleurke.Rood = deKleur.Color.R;
                kleurke.Groen = deKleur.Color.G;
                kleurke.Blauw = deKleur.Color.B;
                cirkelsKleuren.Items.Add(kleurke);
                if (kleurke.Naam == "Red")
                {
                    cirkelsKleuren.SelectedItem = kleurke;
                }
            }
        }
        void KaartKiezen()
        {
            Opslaan.IsEnabled = true;
            Afdrukken.IsEnabled = true;
        }
        void GeboortekaartActive()
        {
            convasImg.Source = new BitmapImage(new Uri(@"..\..\..\Images\geboortekaart.jpg", UriKind.Relative));
            KaartKiezen();
        }
        void KerstkaartActive()
        {
            convasImg.Source = new BitmapImage(new Uri(@"..\..\..\Images\kerstkaart.jpg", UriKind.Relative));
            KaartKiezen();
        }
        void EllipseMaker(Color kleur)
        {
            Brush brush = new SolidColorBrush(kleur);
            sleepBal = new Ellipse();
            sleepBal.Width = 40;
            sleepBal.Height = 40;
            sleepBal.Stroke = Brushes.Black;
            sleepBal.StrokeThickness = 4;
            sleepBal.Fill = brush;
            //canvasArea.Children.Add(sleepBal);
            canvacNew.Children.Add(sleepBal);
            //Canvas.SetLeft(sleepBal, 0);
            //Canvas.SetTop(sleepBal, 0);
            sleepBal.MouseMove += exEllipse_MouseMove;
        }

        void EllipseMetKleur()
        {
            Color kleurGl;
            Kleur kl = (Kleur)cirkelsKleuren.SelectedItem;
            if (kl != null) kleurGl = (Color)ColorConverter.ConvertFromString(kl.Naam);
            EllipseMaker(kleurGl);
        }
        private void exEllipse_MouseMove(object sender, MouseEventArgs e)
        {
            Ellipse ff = (Ellipse)sender;

            if (e.LeftButton == MouseButtonState.Pressed)
            {

                DragDrop.DoDragDrop(ff, new DataObject(DataFormats.Serializable, ff), DragDropEffects.Copy);

    
                Point position = ff.PointToScreen(new Point(0d, 0d)),
                 controlPosition = this.PointToScreen(new Point(0d, 0d));
                position.X -= controlPosition.X;
                position.Y -= controlPosition.Y;
                if (position.X > 650 && position.Y > 250 && position.X < 790 && position.Y < 420)
                {
                    canvasArea.Children.Remove(ff);
                };

            }
        }
        private void canvas_DragOver(object sender, DragEventArgs e)
        {
            object data = e.Data.GetData(DataFormats.Serializable);
            if (data is UIElement element)
            {

                Point dropPosition = e.GetPosition(canvasArea);
                Canvas.SetLeft(element, dropPosition.X - 22);
                Canvas.SetTop(element, dropPosition.Y - 22);
            }
        }
        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            IInputElement clickedElement = Mouse.DirectlyOver;
            if (clickedElement is Ellipse)
            {
                sleepBal = clickedElement as Ellipse;
            }
        }

        private void cirkelsKleuren_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EllipseMetKleur();

        }



        private void Kerstkaart_Click(object sender, RoutedEventArgs e)
        {
            KerstkaartActive();
            Nieuw();
        }

        private void GeboorteKaart_Click(object sender, RoutedEventArgs e)
        {
            GeboortekaartActive();
            Nieuw();
        }

        private void RepeatButtonGroter_Click(object sender, RoutedEventArgs e)
        {
            if (TextWens.FontSize < 40)
            {
                TextWens.FontSize++;
                fontsize.Content = TextWens.FontSize;
            }
        }
        private void RepeatButtonKleiner_Click(object sender, RoutedEventArgs e)
        {
            if (TextWens.FontSize > 10)
            {
                TextWens.FontSize--;
                fontsize.Content = TextWens.FontSize;
            }
        }

        private void Nieuw()
        {
            TextWens.Text = string.Empty;
            for (int i = canvasArea.Children.Count - 1; i >= 0; i += -1)
            {
                UIElement Child = canvasArea.Children[i];
                if (Child is Ellipse)
                    canvasArea.Children.Remove(Child);
            };
            statusItem.Content = "nieuwe bon";
            EllipseMetKleur();

            //StatusItem.Content = "nieuwe bon";
            //SaveEnAfdruk(false);

        }




        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Programma afsluiten ?", "Afsluiten", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                e.Cancel = true;
        }
        private void OpenExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.FileName = "";
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Kaart documents |*.txt";
                if (dlg.ShowDialog() == true)
                {
                    using (StreamReader bestand = new StreamReader(dlg.FileName))
                    {
                        String stringPath = bestand.ReadLine();
                        int bolCount = Int32.Parse(bestand.ReadLine());
                        Uri imageUri = new Uri(stringPath, UriKind.Absolute);
                        BitmapImage imageBitmap = new BitmapImage(imageUri);
                        convasImg.Source = imageBitmap;
                        for (int i = 0; i < bolCount; i++)
                        {
                            Color kleur = (Color)ColorConverter.ConvertFromString(bestand.ReadLine());
                            Brush brush = new SolidColorBrush(kleur);
                            sleepBal = new Ellipse();
                            sleepBal.Width = 40;
                            sleepBal.Height = 40;
                            sleepBal.Stroke = Brushes.Black;
                            sleepBal.StrokeThickness = 4;
                            sleepBal.Fill = brush;
                            canvasArea.Children.Add(sleepBal);
                            Canvas.SetLeft(sleepBal, double.Parse(bestand.ReadLine()));
                            Canvas.SetTop(sleepBal, double.Parse(bestand.ReadLine()));
                        };
                        TextWens.Text = bestand.ReadLine();
                        comboBoxFonts.SelectedValue = new FontFamily(bestand.ReadLine());
                        fontsize.Content = bestand.ReadLine();
                        statusItem.Content = dlg.FileName;
                        KaartKiezen();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("openen mislukt : " + ex.Message);
            }
        }


        private void SaveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Kaart documents |*.txt";
                DateTime tijd = DateTime.Now;
                dlg.FileName = TextWens.Text + tijd.ToString().Replace(" ", "_").Replace(":", "").Replace("/", "");


                if (dlg.ShowDialog() == true)
                {
                    using (StreamWriter bestand = new StreamWriter(dlg.FileName))
                    {
                        bestand.WriteLine(convasImg.Source.ToString());
                        bestand.WriteLine(canvasArea.Children.Count - 1);
                        for (int i = canvasArea.Children.Count - 1; i >= 0; i += -1)
                        {
                            UIElement el = canvasArea.Children[i];
                            if (el is Ellipse)
                            {
                                Ellipse ee = (Ellipse)el;
                                bestand.WriteLine(ee.Fill.ToString());
                                bestand.WriteLine(Canvas.GetLeft(ee).ToString());
                                bestand.WriteLine(Canvas.GetTop(ee).ToString());
                            }
                        };
                        bestand.WriteLine(TextWens.Text);
                        bestand.WriteLine(comboBoxFonts.SelectedItem.ToString());
                        bestand.WriteLine(fontsize.Content);
                        statusItem.Content = dlg.FileName;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("opslaan mislukt : " + ex.Message);
            }
        }

        private void CloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void NewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Nieuw();
        }

        private void PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            FixedDocument document = new FixedDocument();
            document.DocumentPaginator.PageSize = new Size(500, 500);
            PageContent inhoud = new PageContent();
            document.Pages.Add(inhoud);
            FixedPage pagina = new FixedPage();
            inhoud.Child = pagina;
            pagina.Width = 500;
            pagina.Height = 500;
            Image kaart = new Image();
            kaart.Source = convasImg.Source;
            kaart.Height = 430;
            kaart.Width = 430;
            pagina.Children.Add(kaart);
            for (int i = canvasArea.Children.Count - 1; i >= 0; i += -1)
            {
                UIElement el = canvasArea.Children[i];
                if (el is Ellipse)
                {
                    Ellipse eli = new Ellipse();
                    Ellipse ee = (Ellipse)el;
                    eli.Width = 40;
                    eli.Height = 40;
                    eli.Stroke = Brushes.Black;
                    eli.StrokeThickness = 4;
                    eli.Fill = ee.Fill;
                    eli.Margin = new Thickness(Canvas.GetLeft(ee)-40, Canvas.GetTop(ee)-4, 0, 0);
                    pagina.Children.Add(eli);
                };
            }
            pagina.Children.Add(Regel(TextWens.Text));

            TextBlock deRegel = new TextBlock();
            Afdrukvoorbeeld preview = new Afdrukvoorbeeld();
            preview.Owner = this;
            preview.AfdrukDocument = document;
            preview.ShowDialog();

            Regel(TextWens.Text);
        }
        private TextBlock Regel(string tekst)
        {
            TextBlock deRegel = new TextBlock();
            deRegel.Margin = new Thickness(0, 450, 0, 0);
            deRegel.FontSize = TextWens.FontSize;
            deRegel.FontFamily = TextWens.FontFamily;
            deRegel.Text = tekst;
            deRegel.TextAlignment = TextWens.TextAlignment;
            deRegel.Width = 400;
            return deRegel;
        }

        private void canvasArea_Drop(object sender, DragEventArgs e)
        {
            object data = e.Data.GetData(DataFormats.Serializable);
            if(data is UIElement element)
            {
                Point dropPosition = e.GetPosition(canvasArea);
                Canvas.SetLeft(element, dropPosition.X-22);
                Canvas.SetTop(element, dropPosition.Y-22);
                if (!canvasArea.Children.Contains(element)) {
                    canvacNew.Children.Remove(element);
                    canvasArea.Children.Add(element);
                }
            }
        }
        private void canvasNew_Drop(object sender, DragEventArgs e)
        {
            object data = e.Data.GetData(DataFormats.Serializable);
            if (data is UIElement element)
            {
                Point dropPosition = e.GetPosition(canvacNew);
                Canvas.SetLeft(element, dropPosition.X - 22);
                Canvas.SetTop(element, dropPosition.Y - 22);
                if (!canvasArea.Children.Contains(element))
                {
                    canvacNew.Children.Remove(element);

                }
            }
        }
        private void canvacNew_DragLeave(object sender, DragEventArgs e)
        {
            if(e.OriginalSource == canvacNew) { 
            object data = e.Data.GetData(DataFormats.Serializable);
            if (data is UIElement element)
            {
                    EllipseMetKleur();
                    canvasArea.Children.Add(element);
                    
            }
            }
        }
    }
}
