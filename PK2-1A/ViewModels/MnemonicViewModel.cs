﻿using NLog;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Threading;
using cip_blue.Helpers;
using cip_blue.Models;
using cip_blue.Repositories;
using cip_blue.Services;
using Xceed.Wpf.Toolkit;
using System.Net.NetworkInformation;
using System.Reflection;
using Microsoft.Xaml.Behaviors.Core;
using Newtonsoft.Json;
using System.IO;

namespace cip_blue.ViewModels
{
    public class MnemonicViewModel : BindableBase
    {
        private Logger logger = LogManager.GetCurrentClassLogger();

        private bool initVM;
        private PeriodicalTaskStarter chartUpdater;
        private PeriodicalTaskStarter internalUpdater;

        private readonly ArchivRepository archivRepository;
        public ChartData ChartData { get; set; }





        private ObservableRangeCollection<ThermoChartPoint> points;
        public ObservableRangeCollection<ThermoChartPoint> Points
        {
            get { return points; }
            set { SetProperty(ref points, value); }
        }

        private WindowState waterLoadingWndStatus = WindowState.Closed;
        public WindowState WaterLoadingWndStatus
        {
            get { return waterLoadingWndStatus; }
            set { SetProperty(ref waterLoadingWndStatus, value); }
        }

        private WindowState hotwaterLoadingWndStatus = WindowState.Closed;
        public WindowState HotWaterLoadingWndStatus
        {
            get { return hotwaterLoadingWndStatus; }
            set { SetProperty(ref hotwaterLoadingWndStatus, value); }

        }

        private WindowState hot480waterLoadingWndStatus = WindowState.Closed;

        public WindowState Hot480WaterLoadingWndStatus
        {
            get { return hot480waterLoadingWndStatus; }
            set { SetProperty(ref hot480waterLoadingWndStatus, value); }
        }
        // выгрузка r422
        private WindowState unloadFromR422WndStatus = WindowState.Closed;
        public WindowState UnloadFromR422WndStatus
        {
            get { return unloadFromR422WndStatus; }
            set { SetProperty(ref unloadFromR422WndStatus, value); }
        }

        // охлаждение 480
        private WindowState ohlagd480WndStatus = WindowState.Closed;
        public WindowState Ohlagd480WndStatus
        {
            get { return ohlagd480WndStatus; }
            set { SetProperty(ref ohlagd480WndStatus, value); }

        }


        // PH 480a
        private WindowState regPh480aWndStatus = WindowState.Closed;
        public WindowState RegPh480aWndStatus
        {
            get { return regPh480aWndStatus; }
            set { SetProperty(ref regPh480aWndStatus, value); }
        }


        // PH 480b
        private WindowState regPh480bWndStatus = WindowState.Closed;
        public WindowState RegPh480bWndStatus
        {
            get { return regPh480bWndStatus; }
            set { SetProperty(ref regPh480bWndStatus, value); }
        }

        //Морфолин в 480

        private WindowState zagrMorfolin480WndStatus = WindowState.Closed;
        public WindowState ZagrMorfolin480WndStatus
        {
            get { return zagrMorfolin480WndStatus; }
            set { SetProperty(ref zagrMorfolin480WndStatus, value); }
        }

        //Диэтил в 480

        private WindowState zagrDietil480WndStatus = WindowState.Closed;
        public WindowState ZagrDietil480WndStatus
        {
            get { return zagrDietil480WndStatus; }
            set { SetProperty(ref zagrDietil480WndStatus, value); }
        }

        //ДиэтилАмин в 480

        private WindowState zagrDietilAmin480WndStatus = WindowState.Closed;
        public WindowState ZagrDietilAmin480WndStatus
        {
            get { return zagrDietilAmin480WndStatus; }
            set { SetProperty(ref zagrDietilAmin480WndStatus, value); }
        }

        //анилин в 480

        private WindowState zagrAnilin480WndStatus = WindowState.Closed;
        public WindowState ZagrAnilin480WndStatus
        {
            get { return zagrAnilin480WndStatus; }
            set { SetProperty(ref zagrAnilin480WndStatus, value); }
        }


        //термоцикл
        private WindowState thermoCycl_1AWndStatus = WindowState.Closed;
        public WindowState ThermoCycl_1AWndStatus
        {
            get { return thermoCycl_1AWndStatus; }
            set { SetProperty(ref thermoCycl_1AWndStatus, value); }
        }

        // счетчик воды
        private Single count_emis_A;
        public Single Count_emis_A
        {
            get { return count_emis_A; }
            set { SetProperty(ref count_emis_A, value); }
        }

        // счетчик воды
        private bool reset_A = true;
        public bool Reset_A
        {
            get { return reset_A; }
            set { SetProperty(ref reset_A, value); }
        }
        // счетчик воды
        private Single count_emis_B;
        public Single Count_emis_B
        {
            get { return count_emis_B; }
            set { SetProperty(ref count_emis_B, value); }
        }

        private bool reset_B=true;
        public bool Reset_B
        {
            get { return reset_B; }
            set { SetProperty(ref reset_B, value); }
        }



        private bool man_mod = To_Config.ReadRetaneBool("Man_mode");
        public bool Man_mode
        {
            get { return man_mod; }
            set { SetProperty(ref man_mod, value); To_Config.WriteBoolRetane(value, "Man_mode"); }
        }


        // счетчик Мэк 450
        private Single сount_fq450_mek;
        public Single Count_fq450_mek
        {
            get { return сount_fq450_mek; }
            set { SetProperty(ref сount_fq450_mek, value); }
        }

        private bool alarm_notanswerModule = true;
        public bool Alarm_notanswerModule
        {
            get { return alarm_notanswerModule; }
            set { SetProperty(ref alarm_notanswerModule, value); }
        }

        private string status_4101="0" ;
        public string Status_4101
        {
            get { return status_4101; }
            set { SetProperty(ref status_4101, value); }
        }

        private string status_load160a = "0";
        public string Status_load160a
        {
            get { return status_load160a; }
            set { SetProperty(ref status_load160a, value); }
        }


        private string status_4201 = "0";
        public string Status_4201
        {
            get { return status_4201; }
            set { SetProperty(ref status_4201, value); }
        }

        private ProcessDataTcp _pd;
        public ProcessDataTcp PD
        {
            get { return _pd; }
            set { SetProperty(ref _pd, value); }
        }

        private bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        public DelegateCommand Promivka_4101_StartCommand { get; private set; }
        public DelegateCommand Promivka_4101_StopCommand { get; private set; }
        private WindowState promivka_4101WndStatus = WindowState.Closed;
        public WindowState Promivka_4101WndStatus
        {
            get { return promivka_4101WndStatus; }
            set { SetProperty(ref promivka_4101WndStatus, value); }
        }

        /// <summary>
        /// Pfuheprf djls 
        /// </summary>
        public DelegateCommand ZagrVodi160a_StartCommand { get; private set; }
        public DelegateCommand  ZagrVodi160a_StopCommand { get; private set; }
        private WindowState  zagrVodi160aWndStatus = WindowState.Closed;
        public WindowState  ZagrVodi160aWndStatus
        {
            get { return zagrVodi160aWndStatus; }
            set { SetProperty(ref zagrVodi160aWndStatus, value); }

        }

        public DelegateCommand Promivka_4201_StartCommand { get; private set; }
        public DelegateCommand Promivka_4201_StopCommand { get; private set; }

        private WindowState promivka_4201WndStatus = WindowState.Closed;
        public WindowState Promivka_4201WndStatus
        {
            get { return promivka_4201WndStatus; }
            set { SetProperty(ref promivka_4201WndStatus, value); }
        }


        public Dictionary<int, string> Dictionary_Status;
        public MnemonicViewModel(ProcessDataTcp pd, ArchivRepository archivRepository, ChartData chartData)
        {

            PD = pd;
            this.archivRepository = archivRepository;

            Promivka_4101_StartCommand = new DelegateCommand(promivka_4101_Start, canPromivka_4101_Start);
            Promivka_4101_StopCommand = new DelegateCommand(promivka_4101_Stop, canPromivka_4101_Stop);

            ZagrVodi160a_StartCommand = new DelegateCommand(ZagrVodi160a_Start, canZagrVodi160a_Start);
            ZagrVodi160a_StopCommand = new DelegateCommand(ZagrVodi160a_Stop, canZagrVodi160a_Stop);

            Promivka_4201_StartCommand = new DelegateCommand(promivka_4201_Start, canPromivka_4201_Start);
            Promivka_4201_StopCommand = new DelegateCommand(promivka_4201_Stop, canPromivka_4201_Stop);

            Dictionary_Status = new Dictionary<int, string>() {
                {0, "НЕ бЫЛО ЗАПУСКА"},
                { 1, "Цикл промывки:1. Клапан воды открыт. Ожидание таймера на закрытие" },
                { 2, "Цикл промывки:2. Клапан воды закрыт. Ожидание таймера паузы 1" },
                { 3, "Цикл промывки:3. Клапан воздуха открыт. Ожидание таймера на закрытие" },
                { 4, "Цикл промывки:4. Клапан воздуха закрыт. Ожидание таймера на перезапуск" },
                { 5, "5. Финальная продувка воздухом" },
                { 7, "Завершение по условию" },
                 { 10, "Условие выполнено" },
                { 11, "Остановлено оператором" } };


            chartUpdater = new PeriodicalTaskStarter(TimeSpan.FromSeconds(1));
            internalUpdater = new PeriodicalTaskStarter(TimeSpan.FromSeconds(1));
            ChartData = chartData;

            string _fileName = "ChartData.json";
            string jsonString = "";

       
                //using StreamReader data = File.OpenText(_fileName);
                //try
                //{


                //    string readData = data.ReadToEnd();
                //    ChartData? convertedData = JsonConvert.DeserializeObject<ChartData>(readData);
                //    ChartData.DataPoints = convertedData?.DataPoints;
                //    ChartData.DataPoints2 = convertedData?.DataPoints2;

                //}
                //catch (Exception ex)
                //{
                //    ChartData.DataPoints = new();
                //    ChartData.DataPoints2 = new();
                //    // _logger.Error("Error in ChartUpdate Service >>>> " + ex.Message);
                //}
            }

        private bool canPromivka_4101_Start() { return !PD.switch_promivka4101; }
        private void promivka_4101_Start() => PD.switch_promivka4101 = true;
        private bool canPromivka_4101_Stop() { return PD.switch_promivka4101; }
        private void promivka_4101_Stop() => PD.switch_promivka4101 = false;

        private bool canZagrVodi160a_Start() { return !PD.SW_water160a; }
        private void  ZagrVodi160a_Start() => PD.SW_water160a = true;
        private bool canZagrVodi160a_Stop() { return PD.SW_water160a; }
        private void ZagrVodi160a_Stop() => PD.SW_water160a = false;

        private bool canPromivka_4201_Start() { return !PD.switch_promivka4201; }
        private void promivka_4201_Start() => PD.switch_promivka4201 = true;
        private bool canPromivka_4201_Stop() { return PD.switch_promivka4201; }
        private void promivka_4201_Stop() => PD.switch_promivka4201 = false;


        Single mem_count1;
        Single mem_count2;
        Single mem_count3;
        public void OnLoading()
        {
            if (!initVM)
            {
                var t = new Thread(() =>
                {
                    IsBusy = true;

                    internalUpdater.Start(() => internalUpdate(), null);

                    List<ThermoChartPoint> _points = new List<ThermoChartPoint>();
                    DateTime dt = DateTime.MinValue;

                    var dataPoints = archivRepository.GetMeasurements(DateTime.Now.AddHours(-5), DateTime.Now);
                    if (dataPoints == null) // повторяем запрос если возникло исключение
                        dataPoints = archivRepository.GetMeasurements(DateTime.Now.AddHours(-5), DateTime.Now);



                    //foreach (KeyValuePair<DateTime, Dictionary<string, string>> entry in dataPoints)
                    //{
                    //    try
                    //    {
                    //        _points.Add(new ThermoChartPoint()
                    //        {
                    //            DTS = entry.Key,
                    //            TE_1_1A = entry.Value.ContainsKey("TE_1_1A") ? float.Parse(entry.Value["TE_1_1A"], CultureInfo.InvariantCulture) : float.NaN,
                    //            TE_1_1A = entry.Value.ContainsKey("TE_1_1A") ? float.Parse(entry.Value["TE_1_1A"], CultureInfo.InvariantCulture) : float.NaN,
                    //            TE_2_1A = entry.Value.ContainsKey("TE_2_1A") ? float.Parse(entry.Value["TE_2_1A"], CultureInfo.InvariantCulture) : float.NaN,
                    //            TE_3_1A = entry.Value.ContainsKey("TE_3_1A") ? float.Parse(entry.Value["TE_3_1A"], CultureInfo.InvariantCulture) : float.NaN,
                    //            TE_4_1A = entry.Value.ContainsKey("TE_4_1A") ? float.Parse(entry.Value["TE_4_1A"], CultureInfo.InvariantCulture) : float.NaN
                    //        });
                    //        dt = entry.Key;
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        logger.Error(ex, this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
                    //    }

                    //}

                    Points = new ObservableRangeCollection<ThermoChartPoint>(_points);

                    //  chartUpdater.Start(() => updateChart(), null);

                    IsBusy = false;
                });

                t.Priority = ThreadPriority.Lowest;
                t.IsBackground = true;
                t.Start();

                t = null;
                initVM = true;
            }

        }
        public static bool PingHost(string nameOrAddress)
        {
            bool pingable = false;
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(nameOrAddress);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            return pingable;
        }

        private void updateChart()
        {
            //App.Current.Dispatcher?.Invoke(() =>
            //{
            //    Points.Add(new ThermoChartPoint()
            //    {
            //        DTS = DateTime.Now,
            //        TE_1_1A = PD.TE_1_1A,
            //        TE_2_1A = PD.TE_2_1A,
            //        TE_3_1A = PD.TE_3_1A,
            //        TE_4_1A = PD.TE_4_1A
            //    });

            //    IEnumerable<ThermoChartPoint> deletedItem = (from p in Points where p.DTS < DateTime.Now.AddHours(-5) select p).ToList();
            //    if (deletedItem.Count() > 0)
            //        Points.RemoveRange(deletedItem);

            //});
        }
        Single save=0;
        


        private void internalUpdate()
        {
        //    PingHost("192.168.101.117");
            Alarm_notanswerModule = PD.err_module_ad2 || PD.err_module_ad3 || PD.err_module_ad4 || PD.err_module_ad5 || PD.err_module_ad6 || PD.err_module_ad7 || PD.ttyS3_err_module_ad1 || PD.ttyS3_err_module_ad2;

            Promivka_4101_StartCommand.RaiseCanExecuteChanged();
            Promivka_4101_StopCommand.RaiseCanExecuteChanged();

            ZagrVodi160a_StartCommand.RaiseCanExecuteChanged();
            ZagrVodi160a_StopCommand.RaiseCanExecuteChanged();

            Promivka_4201_StartCommand.RaiseCanExecuteChanged();
            Promivka_4201_StopCommand.RaiseCanExecuteChanged();

            try
            {
               
               Status_4101 = Dictionary_Status[Convert.ToInt32(PD.cip4101_status)];
                Status_4201 = Dictionary_Status[Convert.ToInt32(PD.cip4201_status)];
            }
            catch(Exception ex)
            {
               // logger.Error(ex, this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
            }
            if (PD.Load_water_160a_status == 1)
                Status_load160a = "Идет Загрузка Воды";
            else if (PD.Load_water_160a_status == 22)
                Status_load160a = "Загрузка завершена по условию";
            else if (PD.Load_water_160a_status == 20)
                Status_load160a = "Не введена доза, введите дозу";
            else if (PD.Load_water_160a_status == 21)
                Status_load160a = "Загрузка остановлена оператором";
            else if (PD.Load_water_160a_status == 0)
                Status_load160a = "Не было действий";


        }

        ~MnemonicViewModel()
        {
            internalUpdater.Stop();
            // chartUpdater.Stop();
        }
    }


}
