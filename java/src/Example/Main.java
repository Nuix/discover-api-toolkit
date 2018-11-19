package Example;

import com.Ringtail.api.Client;
import com.Ringtail.api.Configuration;

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
            variables.put("name", "APAC");

            Client client = new Client(config);
            String result = client.Execute(query, operation, variables);

            System.out.println(result);
        }
        catch (Exception ex) {
            System.out.println(ex.toString());
        }
    }
}
