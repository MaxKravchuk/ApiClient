using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using WebApplication2.Models;

namespace Lab6_2_CC
{
    public class Menu
    {
        private readonly string path;
        readonly HttpClient HttpClient;

        public Menu(string path, HttpClient httpClient)
        {
            HttpClient = httpClient;
            this.path = path;
        }

        public void MenuInit()
        {
            while (true)
            {
                Console.WriteLine("Press any button to continue");
                Console.ReadKey();
                Console.Clear();
                Console.WriteLine("Commands:\n" +
                    "1 - Sign up\n" +
                    "2 - Make post\n" +
                    "3 - Get all posts\n" +
                    "4 - Get all guests\n" +
                    "5 - Delete guest\n" +
                    "6 - Delete post\n" +
                    "8 - Exit");

                int command;
                do
                {
                    Console.Write("Enter a command number or exit: ");
                    command = Convert.ToInt32(Console.ReadLine());
                }
                while (command < 0 || command > 7);

                switch (command)
                {
                    case 1:
                        try
                        {
                            SignUp();
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        break;
                    case 2:
                        try
                        {
                            MakePost();
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        break;
                    case 3:
                        try
                        {
                            GetAllPosts();
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        break;
                    case 4:
                        try
                        {
                            GetAllGuests();
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        break;
                    case 5:
                        try
                        {
                            DeleteGuest();

                        }
                        catch(Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        break;
                    case 6:
                        try
                        {
                            DeletePost();
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        break;
                    case 7:
                        Environment.Exit(0);
                        break;
                }
            }
        }

        private void SignUp()
        {
            Console.Write("Enter login -> ");
            var login = Console.ReadLine();
            Console.Write("Enter password -> ");
            var pass = Console.ReadLine();
            if (login.Length == 0 && pass.Length == 0) throw new ArgumentNullException(new string("login or pass error"));
            if (Exist(login))
            {
                throw new ArgumentException(new string($"User with login {login} already exists"));
            }
            Console.Write("Enter role (Manager or Guest) -> ");
            var role = Console.ReadLine();
            if (role is not ("Manager" or "Guest")) throw new ArgumentException(new string("role error"));
            var guest = new
            {
                login = login,
                passwordHash = pass,
                guestRole = role
            };
            var res = HttpClient.PostAsJsonAsync(path + "/guest/", guest).Result;
            Console.WriteLine(res.StatusCode.ToString());
        }

        private void MakePost()
        {
            Console.Write("Enter login -> ");
            var login = Console.ReadLine();
            Console.Write("Enter password -> ");
            var pass = Console.ReadLine();
            if(isGuest(login))
            {
                throw new ArgumentException("Login error");
            }
            Console.WriteLine("Enter news header:");
            var header = Console.ReadLine();
            Console.WriteLine("Enter news body:");
            var body = Console.ReadLine();
            Console.WriteLine("Enter news tags:");
            var tags = Console.ReadLine();
            Console.WriteLine("Enter news rubric:");
            var rubric = Console.ReadLine();
            Console.WriteLine("Enter new topic:");
            var topic = Console.ReadLine();
            DateTime time = DateTime.Now.Date;

            var post = new
            {
                guestLogin = login,
                newsHeader = header,
                newsBody = body,
                tags = tags,
                rubric = rubric,
                topic = topic,
                dateTime = time
            };

            var res = HttpClient.PostAsJsonAsync(path + "/post/", post).Result;
            Console.WriteLine(res.StatusCode.ToString());

        }

        private void GetAllPosts()
        {
            var posts = HttpClient.GetAsync(path + "/post").Result.Content.ReadAsAsync<List<PostVm>>().Result;
            foreach (var x in posts)
            {
                Console.WriteLine($"News id {x.Id}");
                Console.WriteLine($"Author {x.guestLogin} Time {x.DateTime}");
                Console.WriteLine($"Tags: {x.Tags}\nTopic: {x.Topic}\nRubric: {x.Rubric}");
                Console.WriteLine($"Header {x.NewsHeader}\nText:\n{x.NewsBody}");
            }
        }

        private void GetAllGuests()
        {
            Console.Write("Enter login -> ");
            var login = Console.ReadLine();
            Console.Write("Enter password -> ");
            var pass = Console.ReadLine();

            if (isAdmin(login)) throw new ArgumentException(new string("Manager error"));
            var guests = HttpClient.GetAsync(path + "/guest").Result.Content.ReadAsAsync<List<GuestVm>>().Result;
            foreach (var x in guests)
            {
                Console.WriteLine($"Login {x.Login}, Role {x.GuestRole}");
            }
        }

        private void DeleteGuest()
        {
            Console.Write("Enter login -> ");
            var login = Console.ReadLine();
            Console.Write("Enter password -> ");
            var pass = Console.ReadLine();

            if (isAdmin(login)) throw new ArgumentException("Error");

            Console.Write("Enter guest login -> ");
            string gLogin = Console.ReadLine();

            var res = HttpClient.DeleteAsync(path + "/guest/" + gLogin).Result;
            Console.WriteLine(res.ToString());
        }

        private void DeletePost()
        {
            GetAllPosts();

            Console.Write("Enter login -> ");
            var login = Console.ReadLine();
            Console.Write("Enter password -> ");
            var pass = Console.ReadLine();

            if (isAdmin(login)) throw new ArgumentException("Error");

            Console.Write("Enter post id -> ");
            var id = Convert.ToInt32(Console.ReadLine());

            var res = HttpClient.DeleteAsync(path + "/post/" + id).Result;
            Console.WriteLine(res.ToString());
        }

        private bool isAdmin(string login)
        {
            var guests = HttpClient.GetAsync(path + "/guest").Result.Content.ReadAsStringAsync().Result;
            if ((guests.Contains($"\"login\":\"{login}\"")) && (guests.Contains($"\"guestRole\":\"Manager\""))) return false;
            return true;
        }

        private bool isGuest(string login)
        {
            var guests = HttpClient.GetAsync(path + "/guest").Result.Content.ReadAsStringAsync().Result;
            if (guests.Contains($"\"login\":\"{login}\"")) return false;
            return true;
        }

        private bool Exist(string login)
        {
            var req = HttpClient.GetAsync(path + "/guest").Result;
            var guests = req.Content.ReadAsStringAsync().Result;
            if(guests.Contains($"\"login\":\"{login}\"")) return true;
            return false;
        }
    }
}
