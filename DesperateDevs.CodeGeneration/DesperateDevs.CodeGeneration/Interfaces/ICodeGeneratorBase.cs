﻿namespace DesperateDevs.CodeGeneration {

    public interface ICodeGeneratorBase {

        string name { get; }
        int priority { get; }
        bool runInDryMode { get; }
    }
}