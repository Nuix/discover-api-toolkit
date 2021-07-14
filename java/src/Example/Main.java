package Example;

import com.Discover.apitoolkit.Client;
import com.Discover.apitoolkit.Configuration;

import java.util.HashMap;
import java.util.Map;

public class Main {
    public static void main(String[] args) {
        System.out.println(Configuration.ConfigurationPath());
        try {
            // TODO: Let the user specify a config or pass in configuration settings.
            Configuration config = Configuration.Load("default");

            String query = "query NamedCases($name: String) { cases(name: $name) { id name} }";
            String operation = "NamedCases";
            Map<String, Object> variables = new HashMap<String, Object>();
            variables.put("name", "Clean_Enron");

            Client client = new Client(config);
            String result = client.Execute(query, operation, variables);

            System.out.println(result);
        }
        catch (Exception ex) {
            System.out.println(ex.toString());
        }
    }
}
