using System;
using System.Collections.Generic;

namespace ex51
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ServiceStation serviceStation = new ServiceStation();

            while (serviceStation.Time != serviceStation.WorkingTime)
            {
                Auto auto = new Auto();

                serviceStation.FixAuto(auto);                
            }

            Console.WriteLine("Рабочий день автосервиса закончен...");
        }
    }

    class DetailShop
    {
        private List<Detail> _details;

        public DetailShop()
        {
            _details = new List<Detail>
            {
                new Detail("Резина", 3000),
                new Detail("Крыло", 2000),
                new Detail("Дверь", 4000),
                new Detail("Бампер", 7000),
                new Detail("Капот", 1000),
                new Detail("Лобовое стекло", 800),
                new Detail("Крыша", 2000),
                new Detail("Зеркало", 200),
                new Detail("Фара", 500),
            };
        }

        public List<Detail> GetDetails()
        {
            return _details;
        }
    }

    class ServiceStation
    {
        const string CommandRejection = "ОТКАЗ";

        private List<Detail> _detailStorage = new List<Detail>();
        private Random _random = new Random();
        private int _detailCount = 10;
        private int _cashBalance = 0;
        private int _penaltyForAbsence = 200;
        private int _penaltyForError = 500;

        public ServiceStation()
        {
            AddDetails();
        }

        public int WorkingTime { get; private set; } = 8;
        public int Time { get; private set; } = 0;

        public void ShowCashBalance()
        {
            Console.SetCursorPosition(50, 0);
            Console.WriteLine($"Зарплата: {_cashBalance} рублей");
            Console.SetCursorPosition(0, 2);
        }

        public void FixAuto(Auto auto)
        {
            bool isFound = TryFindNessesaryDetail(out Detail chosenDetail, auto);
            bool isFixed = false;

            while (isFixed == false)
            {
                if (isFound && chosenDetail.Name == auto.BrokenDetail.Name)
                {
                    int money = auto.RepairPrice + chosenDetail.Price;
                    _detailStorage.Remove(chosenDetail);
                    Console.WriteLine($"Запчасть найдена и переустановлена. Вы получили {money} рублей...");
                    _cashBalance += money;
                    isFixed = true;
                }
                else if (isFound && chosenDetail.Name != auto.BrokenDetail.Name)
                {
                    Console.WriteLine($"Запчасть найдена и переустановлена, но она оказалось не тем, что нужно. С вас штраф: {_penaltyForError} рублей...");
                    _cashBalance -= _penaltyForError;
                    isFixed = true;
                }
                else
                {
                    isFixed = true;
                }

                WorkHour();

                Console.ReadKey();
                Console.Clear();
            }
        }

        private void WorkHour()
        {
            Time++;
        }

        private bool TryFindNessesaryDetail(out Detail chosenDetail, Auto auto)
        {
            bool isWork = true;

            while (isWork)
            {                
                Console.Clear();

                ShowAllInfo(auto);

                List<Detail> storage = ShowStorage();

                Console.Write("\nКакую деталь будем менять? ");
                var chosenOperation = Console.ReadLine();

                if (chosenOperation.ToUpper() == CommandRejection)
                {
                    Console.WriteLine($"Вы отказали клиенту. Штраф: {_penaltyForAbsence} рублей...");
                    _cashBalance -= _penaltyForAbsence;
                    chosenDetail = null;
                    return false;
                }
                else if (int.TryParse(chosenOperation, out int detailNumber))
                {
                    chosenDetail = storage[detailNumber - 1];
                    return true;
                }
                else
                {
                    Console.WriteLine("Неверный ввод. Попробуйте еще раз...");
                    Console.ReadKey();
                }
            }

            chosenDetail = null;
            return false;
        }

        private void ShowAllInfo(Auto auto)
        {
            Console.WriteLine("Автосервис для побитых");
            ShowCashBalance();
            auto.ShowBreakdown();
        }

        private List<Detail> ShowStorage()
        {
            Console.WriteLine("Гараж: ");

            for (int i = 0; i < _detailStorage.Count; i++)
            {
                Console.Write($"{i + 1}. ");
                _detailStorage[i].ShowInfo();
            }

            Console.WriteLine($"\n{CommandRejection} - Отказать клиенту в ремонте");

            return _detailStorage;
        }

        private void AddDetails()
        {
            DetailShop detailShop = new DetailShop();
            List<Detail> details = detailShop.GetDetails();

            for (int i = 0; i < _detailCount; i++)
            {
                _detailStorage.Add(details[_random.Next(0, details.Count)]);
            }
        }
    }

    class Auto
    {
        private Random _random = new Random();

        public Auto()
        {
            DetailShop detailShop = new DetailShop();
            BrokenDetail = GetBreakdown(detailShop.GetDetails());
        }

        public int RepairPrice { get; private set; }
        public Detail BrokenDetail { get; private set; }

        public void ShowBreakdown()
        {
            ConsoleColor defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"Поломка: {BrokenDetail.Name}\n");
            Console.ForegroundColor = defaultColor;
        }

        private Detail GetBreakdown(List<Detail> breakdowns)
        {
            Detail brokenDetail = breakdowns[_random.Next(0, breakdowns.Count)];
            return brokenDetail;
        }
    }

    class Detail
    {
        public Detail(string name, int price)
        {
            Name = name;
            Price = price;
        }

        public int Price { get; private set; }
        public string Name { get; private set; }

        public void ShowInfo()
        {
            Console.WriteLine($"{Name}. Цена: {Price} рублей");
        }
    }
}
