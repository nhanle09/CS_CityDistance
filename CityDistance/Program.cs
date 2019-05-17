using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityDistance
{
    class City
    {
        public string Name { get; } = "None";
        public int Distance { get; }
        public City Parent { get; set; } = null;
        public int Depth { get; set; }
        public int CumulativeDistance { get; set; }

        public City() { }

        public City( string InputCity, int InputDistance )
        {
            Name = InputCity;
            Distance = InputDistance;
            Parent = new City();
        }
    }

    class Program
    {
        // Find Path using modified Breadth-First Search without Heuristic
        static void FindPathUninform( Dictionary<string, LinkedList<City>> list, string start, string goal )
        {
            // Variables
            int node_expanded = 0;
            HashSet<string> explored = new HashSet<string>();
            Queue<City> frontier = new Queue<City>();
            City start_city = new City( start, 0 );

            frontier.Enqueue( start_city );

            // Print out all cities in frontier
            System.Console.WriteLine( "Expanding Node: " + node_expanded );

            while ( frontier.Count != 0 )
            {
                // Displaying current Fringe
                System.Console.WriteLine( "Fringe: " );
                foreach ( City city in frontier )
                {
                    System.Console.WriteLine( "\t" + city.Name + ": g(n) = " + city.CumulativeDistance
                        + ", d = " + city.Depth + ", f(n) = " + city.Parent.Depth 
                        + ", Parent: " + city.Parent.Name );
                }

                // Pop the city in front of frontier queue
                City city_node = frontier.Dequeue();
                node_expanded++;

                // Displaying the city enqueued from frontier
                System.Console.WriteLine( "Generating successor to " + city_node.Name );

                // Goal Condition
                if ( city_node.Name.Equals( goal ) )
                {
                    System.Console.WriteLine( "Node Expanded: " + node_expanded
                        + "\nDistance: " + city_node.CumulativeDistance + "km\nRoute: " );

                    LinkedList<City> solution = new LinkedList<City>();

                    while ( !city_node.Name.Equals( "None" ) )
                    {
                        solution.AddFirst( city_node );
                        city_node = city_node.Parent;
                    }

                    foreach ( City city_i in solution )
                    {
                        System.Console.Write( city_i.Name + " (" + city_i.Distance + " km) >> " );
                    }
                    System.Console.WriteLine();
                    return;
                }

                // Generating successors to non-visited city
                if ( !explored.Contains( city_node.Name ) )
                {
                    explored.Add( city_node.Name );
                    System.Console.WriteLine( "Closed: " );
                    System.Console.Write( "\t" );

                    foreach ( string explored_node in explored )
                    {
                        System.Console.Write( explored_node + ", " );
                    }
                    System.Console.WriteLine();

                    // Generating all successors
                    foreach ( City child_city in list[city_node.Name] )
                    {
                        child_city.Parent = city_node;
                        child_city.CumulativeDistance = child_city.Distance + city_node.CumulativeDistance;
                        child_city.Depth = city_node.Depth + 1;
                        frontier.Enqueue(child_city);

                        // Sort the queue
                        frontier = new Queue<City>( frontier.OrderBy( element => element.CumulativeDistance ) );
                    }
                }
                else
                {
                    System.Console.WriteLine( city_node.Name + " already closed. No successor" );
                }
                System.Console.WriteLine();
            }
        }
        
        // Find path using modified Breadth-First Search with Heuristic
        static void FindPathInform( Dictionary<string, LinkedList<City>> list, string start, string goal, Dictionary<string, int> heuristic )
        {
            // Variables
            int node_expanded = 0;
            HashSet<string> explored = new HashSet<string>();
            Queue<City> frontier = new Queue<City>();
            City start_city = new City(start, 0);
            start_city.CumulativeDistance = heuristic[start_city.Name];

            frontier.Enqueue(start_city);

            // Print out all cities in frontier
            System.Console.WriteLine("Expanding Node: " + node_expanded);

            while (frontier.Count != 0)
            {
                // Displaying Fringe
                System.Console.WriteLine("Fringe: ");
                foreach (City city in frontier)
                {
                    System.Console.WriteLine("\t" + city.Name + ": g(n) = " + city.CumulativeDistance
                        + ", d = " + city.Depth + ", f(n) = " + city.Parent.Depth
                        + ", Parent: " + city.Parent.Name);
                }

                // Pop City in front of frontier queue
                City city_node = frontier.Dequeue();
                city_node.CumulativeDistance -= heuristic[city_node.Name];
                node_expanded++;

                // Displaying the city enqueued from frontier
                System.Console.WriteLine("Generating successor to " + city_node.Name);

                // Goal Condition
                if (city_node.Name.Equals(goal))
                {
                    System.Console.WriteLine("Node Expanded: " + node_expanded
                        + "\nDistance: " + city_node.CumulativeDistance + "km\nRoute: ");

                    LinkedList<City> solution = new LinkedList<City>();

                    while (!city_node.Name.Equals("None"))
                    {
                        solution.AddFirst(city_node);
                        city_node = city_node.Parent;
                    }

                    foreach (City city_i in solution)
                    {
                        System.Console.Write(city_i.Name + " (" + city_i.Distance + " km) >> ");
                    }
                    System.Console.WriteLine();
                    return;
                }

                // Generating successors to non-visited city
                if (!explored.Contains(city_node.Name))
                {
                    explored.Add(city_node.Name);
                    System.Console.WriteLine("Closed: ");
                    System.Console.Write("\t");

                    foreach (string explored_node in explored)
                    {
                        System.Console.Write(explored_node + ", ");
                    }
                    System.Console.WriteLine();

                    // Generating all successors
                    foreach (City child_city in list[city_node.Name])
                    {
                        child_city.Parent = city_node;
                        child_city.CumulativeDistance = 
                            child_city.Distance + city_node.CumulativeDistance + heuristic[child_city.Name];
                        child_city.Depth = city_node.Depth + 1;
                        frontier.Enqueue( child_city );

                        // Sort the queue
                        frontier = new Queue<City>(frontier.OrderBy(element => element.CumulativeDistance));
                    }
                }
                else
                {
                    System.Console.WriteLine(city_node.Name + " already closed. No successor");
                }
                System.Console.WriteLine();
            }
        }

        // Parse import city data into Dictionary
        static Dictionary<string, LinkedList<City>> ImportData( string input_data )
        {
            // Variables
            Dictionary<string, LinkedList<City>> list = new Dictionary<string, LinkedList<City>>();
            string line;

            System.IO.StreamReader file = new System.IO.StreamReader( input_data );
            while( ( line = file.ReadLine() ) != null )
            {
                if ( !line.Equals( "END OF INPUT" ) && !line.Equals( "" ) )
                {
                    LinkedList<City> city_list;
                    string[] line_array = line.Split( ' ' );

                    // Add path from City 1 to 2
                    City city = new City( line_array[1], Int32.Parse( line_array[2] ) );

                    if ( !list.ContainsKey( line_array[0] ) )
                    {
                        city_list = new LinkedList<City>();
                    }
                    else
                    {
                        city_list = list[line_array[0]];
                    }
                    city_list.AddLast( city );
                    list[line_array[0]] = city_list;

                    // Add path from City 2 to 1
                    city = new City( line_array[0], Int32.Parse( line_array[2] ) );
                    if ( !list.ContainsKey( line_array[1] ) )
                    {
                        city_list = new LinkedList<City>();
                    }
                    else
                    {
                        city_list = list[line_array[1]];
                    }
                    city_list.AddLast( city );
                    list[line_array[1]] = city_list;
                }
            }
            // Close file and return parsed dictionary
            file.Close();
            return list;
        }
        
        // Parse import city heuristic into Dictionary
        static Dictionary<string, int> ImportHeuristic( string input_heuristic )
        {
            Dictionary<string, int> heuristic = new Dictionary<string, int>();
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader( input_heuristic );

            while( ( line = file.ReadLine() ) != null )
            {
                if ( !line.Equals( "END OF INPUT" ) && !line.Equals( "" ) )
                {
                    string[] line_array = line.Split(' ');
                    heuristic.Add(line_array[0], Int32.Parse(line_array[1]));
                }
            }
            return heuristic;
        }

        // Main Method
        static void Main( string[] args )
        {
            Dictionary<string, LinkedList<City>> list = ImportData( args[0] );
            
            if ( args.Length == 3 )
            {
                FindPathUninform( list, args[1], args[2] );
            }
            else if ( args.Length == 4 && args[2].Equals( "Kassel" ) )
            {
                Dictionary<string, int> heuristic = ImportHeuristic( args[3] );
                FindPathInform( list, args[1], args[2], heuristic );
            }
            
            System.Console.ReadLine();
        }
    }
}
