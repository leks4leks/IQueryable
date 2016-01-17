using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data.Entity;
using System.Linq.Expressions;

namespace IQueryable
{
    public static class Program
    {
        //Создать таблицу с информацией о пользователях (5-7 различных полей). Случайным образом нагенерировать 100 000 записей
        //Создать приложение с использованием EntityFramework Code First
        //Сделать 5 различных выборок данных максимально используя возможности IQueryable (минимально дублировать код)
        //Запоминая время выполнения запроса сравнить результаты выполнения с использованием IQueryable и c возвратом списка в IEnumerable с последующей фильтрацией с использованием LINQ 
        static void Main(string[] args)
        {
            #region  Create Table
            //using (var db = new myTestTableContext())
            //{
            //    // Create and save a new Table 
            //    Console.Write("Enter a name for a new Blog: ");
            //    var name = Console.ReadLine();

            //    var Table = new myTestTable { Name = name };
            //    db.myTestTable.Add(Table);
            //    db.SaveChanges();

            //    // Display all Tables from the database 
            //    var query = from b in db.myTestTable
            //                orderby b.Name
            //                select b;

            //    Console.WriteLine("All blogs in the database:");
            //    foreach (var item in query)
            //    {
            //        Console.WriteLine(item.Name);
            //    }

            //    Console.WriteLine("Press any key to exit...");
            //    Console.ReadKey();
            //} 
            #endregion
            #region  Fill Table
            //using (var db = new myTestTableContext())
            //{
            //    List<myTestTable> table = new List<myTestTable>();
            //    for (int i = 0; i < 100000; i++)
            //    {
            //        table.Add(new myTestTable
            //                        {
            //                            ID = i,
            //                            Name = "Name" + i,
            //                            FullName = "FullName" + i,
            //                            Age = i % 2 == 0 ? 45 : 25,
            //                            Phone = i,
            //                            CountOfMoney = i / 1006 * 2723
            //                        }
            //        );
            //    }
            //    db.myTestTable.AddRange(table);
            //    db.SaveChanges();
            //}
            #endregion

            using (var db = new myTestTableContext())
            {
                Stopwatch stopWatch = new Stopwatch();
                /// 1 4.7 секунд
                stopWatch.Reset();
                stopWatch.Start();
                var temp1 = db.myTestTable.Where(_ => _.Age == 45 && _.ID % 2 == 0).Select(_ => _.CountOfMoney + 2000).ToList();
                stopWatch.Stop();
                Console.WriteLine("RunTime " + stopWatch.Elapsed);

                stopWatch.Reset();
                /// 2 3.5 секунд
                stopWatch.Start();
                var temp2 = MyWhere(db.myTestTable, _ => _.Age == 45 && _.ID % 2 == 0).Select(_ => _.CountOfMoney + 2000).ToList();
                stopWatch.Stop();
                Console.WriteLine("RunTime " + stopWatch.Elapsed);

                // 3 0.09 секунд
                IQueryable<myTestTable> user = db.myTestTable;
                stopWatch.Reset();
                stopWatch.Start();
                var temp4 = user.Where(_ => _.Age == 45 && _.ID % 2 == 0).Select(_ => _.CountOfMoney + 2000).ToList();
                stopWatch.Stop();
                Console.WriteLine("RunTime " + stopWatch.Elapsed);
            }

            Console.ReadKey();
        }

        public class myTestTableContext : DbContext
        {
            public DbSet<myTestTable> myTestTable { get; set; }
        } 

        public class myTestTable
        {
            public long ID { get; set; }
            public string Name { get; set; }
            public string FullName { get; set; }
            public int Age { get; set; }
            public long Phone { get; set; }
            public decimal CountOfMoney { get; set; }
        }

        public static IEnumerable<T> MyWhere<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (source == null) throw new ArgumentException("source");
            if (predicate == null) throw new ArgumentException("predicate");

            foreach (var tt in source)
            {
                if (predicate(tt))
                    yield return tt;
            }
        }      
    }
}
