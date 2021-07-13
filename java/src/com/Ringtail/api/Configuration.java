package com.Ringtail.api;

import java.io.*;
import java.nio.charset.Charset;
import java.util.List;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.core.type.TypeReference;
import com.fasterxml.jackson.databind.ObjectMapper;
import org.apache.commons.io.FileUtils;

public class Configuration {
    @JsonProperty("name")
    public String Name;
    @JsonProperty("token")
    public String Token;
    @JsonProperty("uri")
    public String Uri;

    public static Configuration Load (String profileName)
            throws IOException {
        File configFile = new File(ConfigurationPath());
        if(! configFile.exists()) {
            throw new FileNotFoundException("Could not find config file");
        }

        String text = FileUtils.readFileToString(configFile, Charset.defaultCharset());

        ObjectMapper mapper = new ObjectMapper();
        try {
            // Single configuration in the file
            Configuration config = mapper.readValue(text, Configuration.class);
            if(config.Name.equals(profileName))
                return config;
            return null;

        } catch(Exception ex) {
            // Array of configuration in the file
            List<Configuration> configs = mapper.readValue(text, new TypeReference<List<Configuration>>(){});

            for(Configuration c : configs) {
                if(c.Name.equals( profileName))
                    return c;
            }
        }

        return null;
    }

    public static String ConfigurationPath() {

        // On Windows, look at the USERPROFILE
        String home = System.getProperty("user.home");

        return home + File.separator + ".ringtail" + File.separator + "config";
    }
}
