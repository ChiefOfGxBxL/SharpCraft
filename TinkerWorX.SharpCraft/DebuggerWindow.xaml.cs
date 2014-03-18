﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Mono.CSharp;
using TinkerWorX.SharpCraft.Blizzard.GameModule;
using TinkerWorX.Utilities;

namespace TinkerWorX.SharpCraft
{
    public partial class DebuggerWindow : Window
    {
        private ConcurrentBagListener listener = new ConcurrentBagListener();

        private Evaluator evaluator;

        public DebuggerWindow(String context, String hackPath)
        {
            this.InitializeComponent();

            Trace.Listeners.Add(this.listener);

            var compilerSettings = new CompilerSettings
            {
                Unsafe = true,
            };
            compilerSettings.AssemblyReferences.Add("System.dll");
            compilerSettings.AssemblyReferences.Add("System.Core.dll");
            compilerSettings.AssemblyReferences.Add(typeof(MessageBox).Assembly.CodeBase); // PresentationFramework.dll
            compilerSettings.AssemblyReferences.Add(Path.Combine(hackPath, "Windows.dll"));
            compilerSettings.AssemblyReferences.Add(Path.Combine(hackPath, "Utilities.dll"));
            compilerSettings.AssemblyReferences.Add(Path.Combine(hackPath, "SharpCraft.dll"));

            var compilerContext = new CompilerContext(compilerSettings, new DebuggerReportPrinter());

            evaluator = new Evaluator(compilerContext);
            evaluator.Compile("using System;");
            evaluator.Compile("using TinkerWorX.Utilities;");
            evaluator.Compile("using TinkerWorX.Windows;");
            evaluator.Compile("using TinkerWorX.SharpCraft;");
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(Object sender, EventArgs e)
        {
            String message;
            while (this.listener.Messages.TryTake(out message))
            {
                this.OutputTextBox.Text += message;
                this.OutputTextBox.ScrollToEnd();
            }
        }

        private void InputTextBox_PreviewKeyDown(Object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                Object result;
                Boolean result_set;
                this.OutputTextBox.Text += this.InputTextBox.Text + " - " + this.evaluator.Evaluate(this.InputTextBox.Text, out result, out result_set) + " -> ";
                if (result_set)
                    this.OutputTextBox.Text += result + Environment.NewLine;
                else
                    this.OutputTextBox.Text += "No result!" + Environment.NewLine;
                this.OutputTextBox.ScrollToEnd();
                this.InputTextBox.Clear();
            }
        }
    }

    internal class DebuggerReportPrinter : ReportPrinter
    {
        public override void Print(AbstractMessage msg, Boolean showFullPath)
        {
            if (msg.IsWarning)
                Trace.WriteLine("Warning: " + msg.Text);
            else
                Trace.WriteLine("Error: " + msg.Text);

            base.Print(msg, showFullPath);
        }
    }
}
