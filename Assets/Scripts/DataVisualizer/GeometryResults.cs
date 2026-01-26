using System.Collections.Generic;

public class Geometry {
    public List<double> DistancesFromLine { get; set; } = new List<double>();
    public List<double> PlaneAxisH { get; set; } = new List<double>();
    public List<double> PlaneAxisV { get; set; } = new List<double>();

    public List<double> Timestamps { get; set; } = new List<double>();
}


public class GeometryResults {
    public Geometry total = new Geometry();
    public Geometry search = new Geometry();
    public Geometry approach = new Geometry();
}
