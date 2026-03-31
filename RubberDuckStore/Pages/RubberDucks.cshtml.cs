using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace RubberDuckStore.Pages
{
    // PageModel for Razor page that handles displaying rubber duck products
    public class RubberDucksModel : PageModel
    {
        // property that will store duck ID of selected duck from form
        [BindProperty]
        public int SelectedDuckId {get; set;}

        // list that will hold all ducks from dropdown list
        public List<SelectListItem> DuckList {get; set;}

        // property that will store currently selected duck object
        public Duck SelectedDuck {get; set;}

        // handles HTTP Get request to page; initializes DuckList from db
        public void OnGet()
        {
            LoadDuckList();
        }

        // handles HTTP Post from form; when user selects a duck
        public IActionResult OnPost()
        {
            // call load duck list method
            LoadDuckList();

            // error handling - ensure there is a valid DuckID
            if (SelectedDuckId != 0)
            {
                SelectedDuck = GetDuckById(SelectedDuckId);
            } 

            // return the page so it can displayed in brower
            return Page();
        } // end OnPost

        // helper method to load ducks from db for displaying in dropdown list
        // private as it is only called within this class
        private void LoadDuckList()
        {
            // create a new duck list
            DuckList = new List<SelectListItem>();

            // create a connection to the SQLite database
            using (var connection = new SqliteConnection("Data Source=Rubber Ducks.db"))
            {
                // open the connection
                connection.Open();

                // create a SQL command and set up SQL query to select all ducks
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Name FROM Ducks";

                // reader
                using (var reader = command.ExecuteReader())
                {
                    // set from the database
                    while (reader.Read())
                    {
                        // create new list item for current duck from result set
                        // add that duck to dropdown list
                        DuckList.Add(new SelectListItem
                        {
                            // duck ID as value & duck Name as text
                            Value = reader.GetInt32(0).ToString(),
                            Text = reader.GetString(1)
                        });
                    } // end while loop
                } // end using reader
            } // end using
        } // end method

        // helper method that retrieves duck via ID from databse; returns duck details
        private Duck GetDuckById(int id)
        {
            // create a sqlite connection
            using (var connection = new SqliteConnection("Data Source=RubberDucks.db"))
            {
                // open a connection
                connection.Open();

                // create command & execute query to retrieve record for selected duck ID
                var command = connection.CreateCommand();

                command.CommandText = "SELECT * FROM Ducks WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", id); // Using parameterized query for security
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Duck
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.GetString(2),
                            Price = reader.GetDecimal(3),
                            ImageFileName = reader.GetString(4)
                        };
                    } // end if
                } // end using
            } // end using
            return null;
        } // end method
    } // end class

    // class representing a Rubber Duck product for store
    public class Duck
    {
        public int Id {get; set;}
        public string Name {get; set;}
        public string Description {get; set;}
        public decimal Price {get; set;}
        public string ImageFileName {get; set;}
    } // end duck class
} // end namespace
