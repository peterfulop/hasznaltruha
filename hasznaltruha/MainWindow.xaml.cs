using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

namespace hasznaltruha
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public int SelectedId { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            SelectedId = 0;
        }


        #region DB METHODS

        private void showGridData()
        {
            using (var db = new VizsgaAdatbazisEntities())
            {
                dg_ruhak.ItemsSource = db.Ruha.Include("Tipus").ToList();
            }
        }

        private int getTipusId(string tipus)
        {
            using (var db = new VizsgaAdatbazisEntities())
            {
                return db.Tipus.FirstOrDefault(x => x.nev == tipus).id;
            }
        }

        private List<string> getMeretList()
        {
            using (var db = new VizsgaAdatbazisEntities())
            {
                return db.Ruha.Include("Tipus").Select(x=>x.meret).Distinct().ToList();
            }
        }
        private List<string> getTipusList()
        {
            using (var db = new VizsgaAdatbazisEntities())
            {
                return db.Ruha.Include("Tipus").Select(x => x.Tipus.nev).Distinct().ToList();
            }
        }
        private List<string> getSzinList()
        {
            using (var db = new VizsgaAdatbazisEntities())
            {
                return db.Ruha.Include("Tipus").Select(x => x.szin).Distinct().ToList();
            }
        }

        private void exportAsMeret(string meret, string path)
        {
            using (var db = new VizsgaAdatbazisEntities())
            {
                var exp = db.Ruha.Where(x => x.meret == meret).ToList();
                exportData(path,exp);
            }
        }
        private void exportAsTipus(string tipus, string path)
        {
            using (var db = new VizsgaAdatbazisEntities())
            {
                var exp = db.Ruha.Where(x => x.Tipus.nev == tipus).ToList();
                exportData(path, exp);
            }
        }
        private void exportAsSzin(string szin,string path)
        {
            using (var db = new VizsgaAdatbazisEntities())
            {
                var exp = db.Ruha.Where(x => x.szin == szin).ToList();
                exportData(path, exp);
            }
        }


        private void searchRuha(string searchFor)
        {
            using (var db = new VizsgaAdatbazisEntities())
            {

                if (radio_keres_tipus.IsChecked.Value)
                {
                    var result = db.Ruha.Include("Tipus").Where(x => x.Tipus.nev.Contains(searchFor)).ToList();

                    if (result.Count()==0)
                    {
                        MessageBox.Show("Nincs találat a típusra!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        dg_ruhak.ItemsSource = result;
                    }
                }
                else
                {
                    var result = db.Ruha.Include("Tipus").Where(x => x.meret.Contains(searchFor)).ToList();
                    if (result.Count() == 0)
                    {
                        MessageBox.Show("Nincs találat a méretre!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        dg_ruhak.ItemsSource = result;
                    }
                }
            }
        }


        private void createRuha(int tipusId, string szin, string meret, int ar)
        {
            using (var db = new VizsgaAdatbazisEntities())
            {
                var ruha = new Ruha()
                {
                    fk_tipus_id = tipusId,
                    szin = szin,
                    meret = meret,
                    ar = ar
                };

                db.Ruha.Add(ruha);
                db.SaveChanges();
            }
            showGridData();
            MessageBox.Show("A létrehozás sikeres volt!", "Művelet kész!", MessageBoxButton.OK, MessageBoxImage.Information);

        }
        private void updateRuha(int tipusId, string szin, string meret, int ar)
        {
            using (var db = new VizsgaAdatbazisEntities())
            {
                var update = db.Ruha.FirstOrDefault(x => x.id == SelectedId);
                update.fk_tipus_id = tipusId;
                update.szin = szin;
                update.meret = meret;
                update.ar = ar;
                db.SaveChanges();
            }
            showGridData();
            MessageBox.Show("A módosítás sikeres volt!", "Művelet kész!", MessageBoxButton.OK, MessageBoxImage.Information);

        }
        private void deleteRuha()
        {
            using (var db = new VizsgaAdatbazisEntities())
            {
                var delete = db.Ruha.FirstOrDefault(x => x.id == SelectedId);
                db.Ruha.Remove(delete);
                db.SaveChanges();
            }
            showGridData();
            MessageBox.Show("A törlés sikeres volt!", "Művelet kész!", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion


        #region GLOBAL METHODS

        private void setDefaultSettings()
        {
            radion_export_szin.IsChecked = true;
        }
        private void setAddOrEditMode(bool add)
        {
            if (add)
            {
                save_btn.Content = "Mentés";
                delete_btn.Visibility = Visibility.Hidden;
                back_btn.Visibility = Visibility.Hidden;
                edit_label_text.Content = "Új tétel hozzáadása";
            }
            else
            {
                save_btn.Content = "Módosítás";
                delete_btn.Visibility = Visibility.Visible;
                back_btn.Visibility = Visibility.Visible;
                edit_label_text.Content = "Tétel módosítása";
            }

        }
        private void setColorExport()
        {
            combo_export_data.Items.Clear();
            getSzinList().ForEach(x =>
            {
                combo_export_data.Items.Add(x);
            });
            combo_export_data.SelectedIndex = 0;
        }
        private void setTipusExport()
        {
            combo_export_data.Items.Clear();
            getTipusList().ForEach(x =>
            {
                combo_export_data.Items.Add(x);
            });
            combo_export_data.SelectedIndex = 0;
        }
        private void setMeretExport()
        {
            combo_export_data.Items.Clear();
            getMeretList().ForEach(x =>
            {
                combo_export_data.Items.Add(x);
            });
            combo_export_data.SelectedIndex = 0;
        }

        private void fillTipusComboBox()
        {
            using (var db = new VizsgaAdatbazisEntities())
            {
                var defaultTipusok =  db.Tipus.Select(x => x.nev).ToList();
                defaultTipusok.ForEach(x =>
                {
                    combo_tipus.Items.Add(x);
                });
                combo_tipus.SelectedIndex = 0;
            }
           
        }

        private void clearCreateInputs()
        {
            combo_tipus.SelectedIndex = 0;
            inp_szin.Clear();
            inp_meret.Clear();
            inp_ar.Clear();
        }

        private void setExportsDefault()
        {
            setColorExport();
        }

        private void exportData(string path, List<Ruha> exportList)
        {
            using (var fs = new FileStream(path, FileMode.Create))
            {
                using (var sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    foreach (var r in exportList)
                    {
                        var line = $"{r.id};{r.Tipus.nev};{r.szin};{r.meret};{r.ar}";
                        sw.WriteLine(line);
                    }
                }
            }
            MessageBox.Show("Az exportálás sikeres volt!", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion


        #region EVENT HANDLERS
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            showGridData();
            setDefaultSettings();
            fillTipusComboBox();
            setExportsDefault();
            setAddOrEditMode(true);
            clearCreateInputs();
        }

        
        private void keres_btn_Click(object sender, RoutedEventArgs e)
        {
            if(inp_keres.Text.Length == 0)
            {
                MessageBox.Show("Nem adtál meg keresési értéket!", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
            else
            {
                searchRuha(inp_keres.Text);
            }
        }
        private void keres_vissza_btn_Click(object sender, RoutedEventArgs e)
        {
            showGridData();
            inp_keres.Clear();
        }


        private void dg_ruhak_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(dg_ruhak.SelectedIndex != -1)
            {
                var selected = (Ruha)dg_ruhak.SelectedItem;
                SelectedId = selected.id;
                combo_tipus.SelectedItem = selected.Tipus.nev;
                inp_szin.Text = selected.szin;
                inp_meret.Text = selected.meret;
                inp_ar.Text = selected.ar.ToString();
                setAddOrEditMode(false);
            }
        }
        private void save_btn_Click(object sender, RoutedEventArgs e)
        {
            var tipus = combo_tipus.SelectedItem.ToString();
            var szin = inp_szin.Text;
            var meret = inp_meret.Text;
            var ar = inp_ar.Text;

            if(string.IsNullOrWhiteSpace(szin) || string.IsNullOrWhiteSpace(meret) || string.IsNullOrWhiteSpace(ar) || string.IsNullOrWhiteSpace(tipus))
            {
                MessageBox.Show("Minden mező kitöltése kötelező!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if(!int.TryParse(inp_ar.Text,out int a))
            {
                MessageBox.Show("Az ár nem szám formátum!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            var tipusId = getTipusId(tipus);

            if (SelectedId == 0)
            {
                createRuha(tipusId, szin, meret, int.Parse(ar));
            }
            else
            {
                var answer = MessageBox.Show("Biztosan módosítani szeretnéd a tételt?", "Figyelem!", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (answer == MessageBoxResult.Yes)
                {
                    updateRuha(tipusId, szin, meret, int.Parse(ar));
                    setAddOrEditMode(true);
                    setExportsDefault();
                }
            }

            clearCreateInputs();
        }
        private void delete_btn_Click(object sender, RoutedEventArgs e)
        {
            var answer = MessageBox.Show("Biztosan törölni szeretnéd a tételt?", "Figyelem!", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if(answer == MessageBoxResult.Yes)
            {
                deleteRuha();
                setAddOrEditMode(true);
                clearCreateInputs();
                setExportsDefault();
            }
        }
        private void back_btn_Click(object sender, RoutedEventArgs e)
        {
            setAddOrEditMode(true);
            clearCreateInputs();
        }


        private void radion_export_szin_Click(object sender, RoutedEventArgs e)
        {
            setColorExport();
        }
        private void radion_export_tipus_Click(object sender, RoutedEventArgs e)
        {
            setTipusExport();
        }
        private void radion_export_meret_Click(object sender, RoutedEventArgs e)
        {
            setMeretExport();
        }
        private void export_btn_Click(object sender, RoutedEventArgs e)
        {

            var dialog = new SaveFileDialog();
            dialog.Filter = "Comma-separated values file (*.csv)|*.csv";
            dialog.ShowDialog();

            if (dialog.FileName.Length > 0)
            {

                var path = dialog.FileName;

                Console.WriteLine(path);

                if (radion_export_szin.IsChecked.Value)
                {
                    exportAsSzin(combo_export_data.SelectedItem.ToString(),path);
                }
                else if (radion_export_tipus.IsChecked.Value)
                {
                    exportAsTipus(combo_export_data.SelectedItem.ToString(),path);
                }
                else if (radion_export_meret.IsChecked.Value)
                {
                    exportAsMeret(combo_export_data.SelectedItem.ToString(),path);
                }
            }
        }
    }

    #endregion


}
