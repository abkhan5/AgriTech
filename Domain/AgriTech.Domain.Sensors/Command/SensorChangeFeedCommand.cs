
using Newtonsoft.Json.Linq;

namespace AgriTech.Domain.Sensors.Command;

public  record SensorChangeFeedCommand(IReadOnlyCollection<JObject> Changes) : ICommand;
