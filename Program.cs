using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ParkingSystem
{
    public class Program
    {
        static void Main(string[] args)
        {
            Menu menu = new Menu();
            menu.initParkSystem();
            do
            {
                var option = menu.mainMenu();
                switch (option)
                {
                    case "1":
                        menu.checkInOption();
                        break;
                    case "2":
                        menu.checkOutOption();
                        break;
                    case "3":
                        string reportOption = menu.reportMenu();
                        menu.reportOption(reportOption);
                        break;
                    case "X" or "x":
                        return;
                    default:
                        Console.WriteLine("Please input the available option");
                        break;
                }
                Console.WriteLine();
            } while (true);
        }
    }

    internal class Menu
    {
        Service service = new Service();
        public void initParkSystem()
        {
            int nLot = 0;
            bool rep = true;
            do
            {
                try
                {
                    Console.Write("Input the capacity of parking system: ");
                    nLot = int.Parse(Console.ReadLine());
                    if (nLot <= 0) throw new Exception();
                    else rep = false;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Input the number of parking capacity\n");
                }
            } while (rep);
            Console.WriteLine();
            service.create_parking_lot(nLot);
        }
        public object mainMenu()
        {
            Console.Write(@"Available parking option:
1. Check-in vehicle
2. Check-out vehicle
3. Report vehicle
X. Exit

Input option: ");
            var option = Console.ReadLine();
            return option;
        }
        public void checkInOption()
        {
            string no = null;
            string pattern = @"^[A-Z]{1,2}-\d{4}-[A-Z]{2,3}$";
            do
            {
                Console.Write("Input vehicle number: ");
                string input = Console.ReadLine();
                if (Regex.IsMatch(input, pattern)) no = input;
                else Console.WriteLine("Input should be in Indonesia vehicle number");
            } while (no is null);
            Console.Write("Input vehicle color: ");
            string color = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Console.ReadLine().ToLower());
            string type = null;
            do
            {
                Console.Write("Input vehicle type: ");
                string input = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Console.ReadLine().ToLower());
                if (input == "Mobil" || input == "Motor") type = input;
                else Console.WriteLine("Parking system only accept Motor or Mobil vehicle");
            } while (type is null);
            Console.WriteLine(service.park(no, color, type));
        }
        public void checkOutOption()
        {
            int slot = 0;
            bool rep = true;
            do
            {
                try
                {
                    Console.Write("Input vehicle slot to remove: ");
                    slot = int.Parse(Console.ReadLine());
                    if (slot <= 0 || slot > service.data.GetLength(0)) throw new Exception();
                    else rep = false;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Vehicle slot must be in range of capacity\n");
                }
            } while (rep);
            Console.WriteLine(service.leave(slot));
        }

        public string reportMenu()
        {
            Console.Write(@"Available report option:
1. Check status
2. Check vehicle type
3. Check ood vehicle number
4. Check event vehicle number
5. Check vehicle number from color
6. Check slot from color
7. Check slot from vehicle number
8. Check slot available

Input option: ");
            var option = Console.ReadLine();
            return option;
        }

        public void reportOption(string option)
        {
            switch (option)
            {
                case "1":
                    service.status(); break;
                case "2":
                    string type = null;
                    do
                    {
                        Console.Write("Input vehicle type: ");
                        string input = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Console.ReadLine().ToLower());
                        if (input == "Mobil" || input == "Motor") type = input;
                        else Console.WriteLine("Parking system only accept Motor or Mobil vehicle");
                    } while (type is null);
                    Console.WriteLine("There is " + service.type_of_vehicles(type) + " " + type + " in parking system"); ; break;
                case "3":
                    Console.WriteLine(service.registration_numbers_for_vehicles_with_ood_plate()); ; break;
                case "4":
                    Console.WriteLine(service.registration_numbers_for_vehicles_with_event_plate()); ; break;
                case "5":
                    Console.Write("Input color: ");
                    string color = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Console.ReadLine().ToLower());
                    Console.WriteLine(service.registration_numbers_for_vehicles_with_colour(color)); ; break;
                case "6":
                    Console.Write("Input color: ");
                    color = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Console.ReadLine().ToLower());
                    Console.WriteLine(service.slot_numbers_for_vehicles_with_colour(color)); break;
                case "7":
                    Console.Write("Input vehicle number: ");
                    string no = Console.ReadLine();
                    Console.WriteLine(service.slot_numbers_for_vehicles_with_colour(no)); break;
                case "8":
                    Console.WriteLine(service.slotAvailable()); break;
                default:
                    Console.WriteLine("Report option unavailable");
                    break;
            }
        }
    }

    internal class Service
    {
        public string[,] data;
        public void create_parking_lot(int nLot)
        {
            data = new string[nLot, 4];
        }
        public string park(string no, string color, string type)
        {
            for (int i = 0; i < data.GetLength(0); i++)
            {
                if (data[i, 1] is null)
                {
                    data[i, 0] = (i + 1).ToString();
                    data[i, 1] = no;
                    data[i, 2] = type;
                    data[i, 3] = color;
                    return "Allocated slot number: " + (i + 1);
                }
            }
            return "Sorry, parking lot is full";
        }
        public string leave(int slot)
        {
            if (data[slot - 1, 1] is null) return "Parking slot " + slot + " is empty, cannot check-out vehicle";
            else
            {
                for (int i = 0; i < data.GetLength(1); i++) data[slot - 1, i] = null;
                return "Slot number " + slot + " is free";
            }
        }
        public void status()
        {
            Console.WriteLine(string.Format("{0,-5} {1,-15} {2,-10} {3,-10}", "Slot", "No", "Type", "Color"));
            for (int i = 0; i < data.GetLength(0); i++)
            {
                if (data[i, 1] is not null) Console.WriteLine(string.Format("{0,-5} {1,-15} {2,-10} {3,-10}", data[i, 0], data[i, 1], data[i, 2], data[i, 3]));
            }
        }
        public int type_of_vehicles(string type)
        {
            int count = 0;
            for (int i = 0; i < data.GetLength(0); i++)
            {
                if (data[i, 2] == type) count++;
            }
            return count;
        }
        public string registration_numbers_for_vehicles_with_ood_plate()
        {
            string odd = "";
            int count = 0;
            for (int i = 0; i < data.GetLength(0); i++)
            {
                int plateNo = int.Parse(data[i, 1].Substring(data[i, 1].IndexOf('-') + 1, 4));
                if (plateNo % 2 == 1)
                {
                    odd = odd + data[i, 1] + ", ";
                    count++;
                }
            }
            if (odd.Length > 0) return odd.Substring(0, odd.Length - 2) + "\nVehicle count with ood plate: " + count;
            return "No vehicle have odd plate number";
        }
        public string registration_numbers_for_vehicles_with_event_plate()
        {
            string even = "";
            int count = 0;
            for (int i = 0; i < data.GetLength(0); i++)
            {
                int plateNo = int.Parse(data[i, 1].Substring(data[i, 1].IndexOf('-') + 1, 4));
                if (plateNo % 2 == 0)
                {
                    even = even + data[i, 1] + ", ";
                    count++;
                }
            }
            if (even.Length > 0) return even.Substring(0, even.Length - 2) + "\nVehicle count with even plate: " + count;
            return "No vehicle have even plate number";
        }
        public string registration_numbers_for_vehicles_with_colour(string color)
        {
            string result = "";
            int count = 0;
            for (int i = 0; i < data.GetLength(0); i++)
            {
                if (data[i, 3] == color)
                {
                    result = result + data[i, 1] + ", ";
                    count++;
                }
            }
            if (result.Length > 0) return result.Substring(0, result.Length - 2) + "\nVehicle count with color " + color + ": " + count;
            return "No vehicle have color " + color;
        }
        public string slot_numbers_for_vehicles_with_colour(string color)
        {
            string result = "";
            for (int i = 0; i < data.GetLength(0); i++)
            {
                if (data[i, 3] == color)
                {
                    result = result + data[i, 0] + ", ";
                }
            }
            if (result.Length > 0) return result.Substring(0, result.Length - 2);
            return "No vehicle have color " + color;
        }
        public string slot_number_for_registration_number(string number)
        {
            string result = "";
            for (int i = 0; i < data.GetLength(0); i++)
            {
                if (data[i, 1] == number)
                {
                    result = result + data[i, 0] + ", ";
                }
            }
            if (result.Length > 0) return result.Substring(0, result.Length - 2);
            return "Not found";
        }
        public string slotAvailable()
        {
            int count = 0;
            string slot = null;
            for (int i = 0; i < data.GetLength(0); i++)
            {
                if (data[i, 1] is null)
                {
                    slot = slot + (i + 1) + ", ";
                    count++;
                }
            }
            if (count > 0) return "Slot available: " + slot.Substring(0, slot.Length - 2) + "\nSlot available count: " + count;
            return "Parking slot is full";
        }
    }
}
