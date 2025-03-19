using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using System.Data;
using static System.Net.Mime.MediaTypeNames;
using System.Threading;
using System.Reflection.PortableExecutable;


namespace TelegramBotForLesson
{
    //Телеграм бот связь с базой данных рабочий
    //регистрация пользователя, запрос списка всех позиций БД
    class Program
    {
        const string TOKEN = "TOKEN";
        const string connectionString = "Server=localhost\\SQLEXPRESS;Database=DB;Trusted_Connection=True;TrustServerCertificate=True;";



        static async Task Main(string[] args)
        {
            while (true)
            {
                try
                {
                    GetMessages().Wait();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error" + ex);
                }
            }
        }

       
        static public async Task GetMessages()
        {
            //string connectionString = "Server=localhost\\SQLEXPRESS;Database=DB;Trusted_Connection=True;TrustServerCertificate=True;";

            TelegramBotClient bot = new TelegramBotClient(TOKEN);
            int offset = 0;
            int timeout = 0;



            //Ввод данных с консоли для регистрации в БД
            //Console.WriteLine("Введите ChatId:");
            //string s1 = Convert.ToString(Console.ReadLine());
            //Console.WriteLine("Введите логин:");
            //string s2 = Convert.ToString(Console.ReadLine());
            //Registration(s1, s2);



            try
            {
                await bot.SetWebhookAsync(""); //отсутствует вебхук
                //мы будем получать обновление и вытаскивать значение бесконечный цикл
                while (true)
                {
                    var updates = await bot.GetUpdatesAsync(offset, timeout);

                    foreach (var update in updates)
                    {
                        var message = update.Message;
                        if (message.Text == "/start2")
                        {
                            var keyboard2 = new ReplyKeyboardMarkup
                            {
                                Keyboard = new[]
                                {
                                    new[]
                                    {
                                        new KeyboardButton(@"/start")
                                    },
                                    new[]
                                    {
                                        new KeyboardButton("Список позиций"),
                                    },
                                }

                            };
                            await bot.SendMessage(message.Chat.Id,
                                "Нажато /start2 , уважаемый пользователь " + message.Chat.Username,
                                ParseMode.Html,
                                0,
                                keyboard2
                            );
                        }
                        if (message.Text == "/start")
                        {
                            Console.WriteLine("Получено сообщение: " + message.Text + " Chat.Id : " + message.Chat.Id + " Username: " + message.Chat.Username.ToString()); //выводит на консоль на компьютере

                            var keyboard = new ReplyKeyboardMarkup
                            {
                                Keyboard = new[]
                                {
                                    new[]
                                    {
                                        //new KeyboardButton("\U0001F601 Привет"),
                                        new KeyboardButton(@"/start2"),
                                        new KeyboardButton(@"/start")
                                    },
                                    new[]
                                    {
                                        new KeyboardButton("Help"),
                                        new KeyboardButton("Регистрация пользователя")
                                    },
                                }

                            };
                            await bot.SendMessage(message.Chat.Id,
                               "Привет, создатель, я твой бот! " + message.Chat.Username,
                                ParseMode.Html,
                                0,
                                keyboard
                            );

                        }

                        if (message.Text == "Help")
                        {
                            Console.WriteLine("Получено сообщение: " + message.Text + " Chat.Id : " + message.Chat.Id + " Username: " + message.Chat.Username.ToString()); //выводит на консоль на компьютере
                            await bot.SendTextMessageAsync(message.Chat.Id, "Нужна помощь, " + message.Chat.Username + "?"); //отправляет сообщение пользователю

                        }
                        if (message.Text == "/dir")
                        {
                            Registration(message.Chat.Id.ToString(), message.Chat.Username.ToString());
                            await bot.SendTextMessageAsync(message.Chat.Id, $"Регистриация пользователь: {message.Chat.Id} ");
                            await bot.SendTextMessageAsync(message.Chat.Id, "Нажато /dir");
                            Console.WriteLine("Нажато /dir");
                        }

                        if (message.Text == "Регистрация пользователя")
                        {
                            Console.WriteLine("Получено сообщение: " + message.Text + ", Chat.Id : " + message.Chat.Id + ", Username: " + message.Chat.Username.ToString());
                            _ = Registration(message.Chat.Id.ToString(), message.Chat.Username.ToString());
                            await bot.SendTextMessageAsync(message.Chat.Id, $"Вы зарегистрированы ChatId: {message.Chat.Id}, Username:  {message.Chat.Username.ToString()}");

                        }
                        if (message.Text == "Список позиций")
                        {
                            //Console.WriteLine("Получено сообщение [из условия if]: " + message.Text + ", Chat.Id : " + message.Chat.Id + ", Username: " + message.Chat.Username.ToString());
                            GetAllTask(message.Chat.Id.ToString(), message.Chat.Username.ToString());
                            Message message1 = await bot.SendTextMessageAsync(message.Chat.Id, $"условие if Запрос полей от ChatId: {message.Chat.Id}, Id:  {message.Chat.Username.ToString()}");
                            await bot.SendTextMessageAsync(message.Chat.Id, $": {message.Chat.Id}, Username:  {message.Chat.Username.ToString()}");


                        }
                        //чтобы не приходило миллион обновлений
                        offset = update.Id + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error" + ex);

            }
        }

        public static async Task GetAllTask(string numberchatId, string numberusername)
        {
            TelegramBotClient bot = new TelegramBotClient(TOKEN);
            string sqlExpression = $"SELECT * FROM RegUsers";
            //int idPrint = 0;

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();
                SqlCommand command = new SqlCommand(sqlExpression, sqlConnection);
                SqlDataReader reader = await command.ExecuteReaderAsync();

                if (reader.HasRows)  //если есть данные
                {
                    //выводим на консоль
                    //string columnName1 = reader[0]         //.ToString();
                    string columnName1 = reader.GetName(0);  //выводим заголовки таблицы
                    string columnName2 = reader.GetName(1);
                    string columnName3 = reader.GetName(2);
                    Console.WriteLine($"Запрос на список заявок метод GetAllTask от chatId: {numberchatId}, username: {numberusername}");
                    Console.WriteLine($"{columnName1}\t{columnName2}\t{columnName2}");

                    //С помощью метода reader.ReadAsync() ридер переходит к следующей строке и возвращает
                    //булевое значение, которое указывает, есть ли данные для считывания.
                    while (await reader.ReadAsync()) //построчно считываем данные
                    {
                        //object id = reader[0].ToString();
                        //object id = reader[0]; //третий вариант
                        //object id = reader.GetValue(0); //в порядке следования столбов получаем данные
                        object id = reader["id"]; //второй вариант получения данных
                        object chatid = reader.GetValue(1);
                        object username = reader.GetValue(2);

                        Console.WriteLine($"{id} \t{chatid} \t{username}");
                        //idPrint = (int)(id);
                        //выводим в телеграм пользователю
                        await bot.SendTextMessageAsync(numberchatId, $"id: {username}, ChatId:  {id}, username: {chatid}");

                    }
                    //После завершения работы с SqlDataReader надо его закрыть методом CloseAsync(). И пока
                    //один SqlDataReader не закрыт, другой объект SqlDataReader для одного и того же подключения мы использовать не сможем.
                    await reader.CloseAsync();

                }

                
            }
           
        }
        public static async Task Registration(string chatId, string username)
        {
            //string connectionString = "Server=localhost\\SQLEXPRESS;Database=DB;Trusted_Connection=True;TrustServerCertificate=True;";
            string sqlExpression = $"INSERT INTO RegUsers VALUES('{chatId}', '{username}')";


            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();
                SqlCommand command = new SqlCommand(sqlExpression, sqlConnection);
                await command.ExecuteNonQueryAsync();
                Console.WriteLine($"Зарегистрирован chatId: {chatId}, username: {username}");
            }
                 


        }
    }
}



//----------------------------------------------------------------------------------------------
//----------------------------------------------------------------------------------------------
//----------------------------------------------------------------------------------------------
//----------------------------------------------------------------------------------------------
//Ниже резервная копия
//system.Data.SQLite.Core --> nuget
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Telegram.Bot;
//using Telegram.Bot.Types;
//using Telegram.Bot.Types.Enums;
//using Telegram.Bot.Types.ReplyMarkups;
//using System.Data.SQLite;
//using System.Data.SqlClient;
//using Microsoft.Data.SqlClient;
//using System.Data;
//using static System.Net.Mime.MediaTypeNames;
//using System.Threading;

//namespace TelegramBotForLesson
//{
//    //Телеграм бот связь с базой данных
//    class Program
//    {
//        const string TOKEN = "7169505210:AAEBnzeSX666BGha_gG3O3l0VaU3zogdCQI";
//        public static SQLiteConnection DB;

//        static void Main(string[] args)
//        {
//            while (true)
//            {
//                try
//                {
//                    GetMessages().Wait();
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine("Error" + ex);
//                }
//            }
//        } 

//        public static async Task GetMessages()
//        {
//            TelegramBotClient bot = new TelegramBotClient(TOKEN);
//            int offset = 0;
//            int timeout = 0;
//            try
//            {
//                await bot.SetWebhookAsync(""); //отсутствует вебхук
//                //мы будем получать обновление и вытаскивать значение бесконечный цикл
//                while (true)
//                {
//                    var updates = await bot.GetUpdatesAsync(offset, timeout);

//                    foreach (var update in updates)
//                    {
//                        var message = update.Message;
//                        if (message.Text == "/start") 
//                        {
//                            Console.WriteLine("Получено сообщение: " + message.Text + " Chat.Id : " + message.Chat.Id + " Username: " + message.Chat.Username.ToString()); //выводит на консоль на компьютере
//                            var keyboard = new ReplyKeyboardMarkup
//                            {
//                                Keyboard = new[]
//                                {
//                                    new[]
//                                    {
//                                        new KeyboardButton("\U0001F601 Привет"),
//                                        new KeyboardButton("Как дела?")
//                                    },
//                                    new[]
//                                    {
//                                        new KeyboardButton("Хай"),
//                                        new KeyboardButton("Как дела?")
//                                    },
// }

//                            };
//                            await bot.SendMessage(message.Chat.Id,
//                               "Привет, создатель, я твой бот!" + message.Chat.Username,
//                                ParseMode.Html,                                                                
//                                0,
//                                keyboard
//                            );

//                        }

//                        if (message.Text == "Help")
//                        {
//                            Console.WriteLine("Получено сообщение: " + message.Text + " Chat.Id : " + message.Chat.Id + " Username: " + message.Chat.Username.ToString()); //выводит на консоль на компьютере
//                            await bot.SendTextMessageAsync(message.Chat.Id, "Привет, создатель, я твой бот!" + message.Chat.Username); //отправляет сообщение пользователю

//                        }
//                        if (message.Text == "/reg")
//                        {
//                            Registration(message.Chat.Id.ToString(), message.Chat.Username.ToString());
//                            await bot.SendTextMessageAsync(message.Chat.Id, "Пользователь зарегистрирован");
//                        }
//                        //чтобы не приходило миллион обновлений
//                        offset = update.Id + 1;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("Error" + ex);

//            }
//        }


//        public static void Registration(string chatId, string username)
//        {

//            try
//            {
//                DB = new SQLiteConnection(@"Server=localhost\SQLEXPRESS;Database=DB;Trusted_Connection=True;");


//                if (DB.State == System.Data.ConnectionState.Closed)
//                {
//                    DB.Open();
//                }


//                SQLiteCommand regcmd = DB.CreateCommand();
//                regcmd.CommandText = "INSERT INTO RegUsers VALUES(@ChatId, @Username)";
//                regcmd.Parameters.AddWithValue("@ChatId", chatId);
//                regcmd.Parameters.AddWithValue("@Username", username);
//                regcmd.ExecuteNonQuery();



//                if (DB.State == System.Data.ConnectionState.Open)
//                {
//                    DB.Close();
//                }

//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("Error: " + ex);

//            }


//        }
//    }
//}
//----------------------------------------------------------------------------------------------------------
//----------------------------------------------------------------------------------------------
//----------------------------------------------------------------------------------------------
//----------------------------------------------------------------------------------------------
