package com.Ringtail.api;

import com.fasterxml.jackson.databind.ObjectMapper;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.net.HttpURLConnection;
import java.net.URL;
import java.util.Map;

public class Client {
    private Configuration configuration;

    public Client (Configuration config) {
        this.configuration = config;

    }

    public String Execute(String query, String operation, Map<String, Object> variables ) throws IOException
    {
        URL url = new URL(configuration.Uri);
        HttpURLConnection connection = (HttpURLConnection) url.openConnection();
        connection.setRequestMethod("POST");
        connection.setRequestProperty("Authorization", "Bearer " + configuration.Token);
        connection.setRequestProperty("ApiKey", configuration.ApiKey);
        connection.setRequestProperty("Content-Type", "application/json");
        connection.setDoInput(true);
        connection.setDoOutput(true);

        GraphQLRequest request = new GraphQLRequest();
        request.Query = query;
        request.OperationName = operation;
        request.Variables = variables;

        ObjectMapper mapper = new ObjectMapper();

        OutputStreamWriter writer = new OutputStreamWriter(connection.getOutputStream());
        mapper.writeValue(writer, request);
        writer.close();

        // int status = connection.getResponseCode();

        BufferedReader in = new BufferedReader(
                new InputStreamReader(connection.getInputStream()));
        String inputLine;
        StringBuffer content = new StringBuffer();
        while ((inputLine = in.readLine()) != null) {
            content.append(inputLine);
        }
        in.close();

        return content.toString();

    }

    class GraphQLRequest {
        public String OperationName;
        public String Query;
        public Map<String, Object>  Variables;
    }
}
