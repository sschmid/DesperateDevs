namespace DesperateDevs.CodeGeneration {

    public enum DiagnosisSeverity {
        Healthy,
        Hint,
        Warning,
        Error
    }

    public class Diagnosis {

        public static Diagnosis Healthy { get { return new Diagnosis(null, null, DiagnosisSeverity.Healthy); } }

        public readonly string symptoms;
        public readonly string treatment;
        public readonly DiagnosisSeverity severity;

        public Diagnosis(string symptoms, string treatment, DiagnosisSeverity severity) {
            this.symptoms = symptoms;
            this.treatment = treatment;
            this.severity = severity;
        }
    }
}
