using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;

namespace SignalRchatConsole
{
    class Program
    {
        static HubConnection connection;
        static string UserName;

        static void Main(string[] args)
        {
            Console.WriteLine("Seja bem vindo!");
            Console.WriteLine("Digite seu nome: ");
            UserName = Console.ReadLine();

            Connect();

            while(true)
            {
                var message = Console.ReadLine();
                SendMessage(message);
            }
        }

        public static async void Connect()
        {
            connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:44393/chat")
                .Build();

            connection.On<string>("newUser", (user) =>
            {
                var message = user == UserName ? "Você entrou na sala" : $"{user} acabou de entrar";
                Console.WriteLine(message);
            });

            connection.On<string, string>("newMessage", (user, message) =>
            {
                if (user != UserName)
                    Console.WriteLine($"{user}: {message}");
            });

            connection.On<List<Message>>("previousMessages", (messages) =>
            {
                foreach (var msg in messages)
                {
                    Console.WriteLine($"{msg.UserName}: {msg.Text}");
                }
            });

            try
            {
                await connection.StartAsync();
                await connection.SendAsync("newUser", UserName, connection.ConnectionId);
            }
            catch
            {
                Console.WriteLine("Ocorreu um erro ao conectar");
            }
        }

        static async void SendMessage(string message)
        {
            try
            {
                await connection.SendAsync("newMessage", UserName, message);
            }
            catch
            {
                Console.WriteLine("Ocorreu um erro ao enviar a mensagem");
            }
        }
    }

    public class Message
    {
        public string UserName { get; set; }
        public string Text { get; set; }
    }
}
