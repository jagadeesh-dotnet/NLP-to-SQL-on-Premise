using System;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class TextToSql
{
    private static readonly HttpClient client = new HttpClient();

    public static async Task<string> GenerateSqlQueryAsync(string naturalQuery)
    {
        // local api url from  flask
        string apiUrl = "http://localhost:5000/generate_sql";

        // Create the JSON payload
        var payload = new
        {
            query = naturalQuery
        };

        string jsonPayload = JsonConvert.SerializeObject(payload);

        // Send the post request to the local flask api
        HttpContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await client.PostAsync(apiUrl, content);

        if (response.IsSuccessStatusCode)
        {
            // Parse the response JSON
            string responseBody = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(responseBody);
            return result.sql;
        }
        else
        {
            return "Error generating SQL query.";
        }
    }


    public static string ConvertOracleToSqlServer(string oracleQuery)
    {
        // Example of basic string replacements
        string sqlServerQuery = oracleQuery;

        // 1. Convert `SYSDATE` to `GETDATE()`
        sqlServerQuery = Regex.Replace(sqlServerQuery, @"\bSYSDATE\b", "GETDATE()", RegexOptions.IgnoreCase);

        // 2. Convert `TO_DATE` to `CONVERT(DATE, ...)`
        sqlServerQuery = Regex.Replace(sqlServerQuery, @"TO_DATE\((.*)\)", m =>
        {
            string dateValue = m.Groups[1].Value;
            return $"CONVERT(DATE, {dateValue}, 120)";
        }, RegexOptions.IgnoreCase);

        // 3. Replace `ROWNUM` or `FETCH FIRST` with SQL Server Pagination (`OFFSET ... FETCH`)
        sqlServerQuery = Regex.Replace(sqlServerQuery, @"FETCH FIRST (\d+) ROWS ONLY", m =>
        {
            int rowCount = int.Parse(m.Groups[1].Value);
            return $"OFFSET 0 ROWS FETCH NEXT {rowCount} ROWS ONLY";
        }, RegexOptions.IgnoreCase);

        // 4. Convert `NVL` to `ISNULL`
        sqlServerQuery = Regex.Replace(sqlServerQuery, @"NVL\(([^,]+),([^)]+)\)", m =>
        {
            string column = m.Groups[1].Value;
            string replacement = m.Groups[2].Value;
            return $"ISNULL({column}, {replacement})";
        }, RegexOptions.IgnoreCase);

        // 5. Handle `ROWNUM` with SQL Server `ROW_NUMBER()` or `TOP` for limiting rows
        sqlServerQuery = Regex.Replace(sqlServerQuery, @"SELECT\s+\*.*\s+WHERE\s+ROWNUM\s*=\s*1", "SELECT TOP 1 *", RegexOptions.IgnoreCase);

        // Additional replacements can be added here for more complex scenarios

        return sqlServerQuery;
    }
    static void Main(string[] args)
    {
        Console.WriteLine("Enter your query. Type 'exit' to quit.\"\r\n");
        Console.WriteLine(" schema:Sales(SalesOrderID, OrderDate, DueDate,OrderStatus,TotalDue, CustomerID,FirstName, LastName, Title, SalesOrderDetailID, OrderQty, LineTotal,ProductID, ProductName,ProductNumber, StandardCost,ListPrice)\"\r\n  ");
        var naturalQuery = "";
        // Loop to continuously take user input
        while (true)
        {
            // Prompt the user for input
            Console.Write("Enter a parameter (or 'exit' to quit): ");
            string userInput = Console.ReadLine();

            // Check if the user wants to exit
            if (userInput.ToLower() == "exit")
            {
                Console.WriteLine("Exiting the program...");
                break;
            }

            naturalQuery = userInput;// +" from employees";//Console.ReadLine();//"Show all employees with salary greater than $50000";
            string sqlQuery = GenerateSqlQueryAsync(naturalQuery).Result;
            //sqlQuery= ConvertOracleToSqlServer(sqlQuery);
            Console.WriteLine("Generated SQL Query: " + sqlQuery);
            GenerateSQLData(sqlQuery);
        }
    }

    private static void GenerateSQLData(string sqlQuery)
    {
        try
        {
            string connectionString = "Data Source=ASUSROG;Initial Catalog=AdventureWorks2022;Integrated Security=True;";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sqlQuery;

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    string data = "";
                    for(var i = 0; i < rdr.FieldCount; i++)
                    {
                        data += rdr[i].ToString() +" ";
                    }
                    Console.WriteLine(data);
                }
            }
        }
        catch (Exception exp)
        {
            Console.WriteLine("Sorry Please try other Query");
        }
        finally
        {
            Console.WriteLine("\r\n");
        }


    }

    //// Example usage
    //public static async Task Main(string[] args)
    //{
    //    try {
    //        var naturalQuery = "";
    //        do
    //        {

    //            Console.WriteLine("please enter your query");
    //            naturalQuery = Console.ReadLine();//"Show all employees with salary greater than $50000";
    //            string sqlQuery = await GenerateSqlQueryAsync(naturalQuery);
    //            Console.WriteLine("Generated SQL Query: " + sqlQuery);
    //            naturalQuery=Console.ReadLine();
    //        }
    //        while (naturalQuery == "exit");

    //    }
    //    catch (Exception e)
    //    {
    //        Console.WriteLine(e.Message);
    //    }
    //}
}
