using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Elements;
using Pulse.Expressions.ActionExpressions;
using Pulse.Expressions.ScalarExpressions;
using System.IO;
using System.IO.Compression;
using System.Net;
using Pulse.Tables;
using Pulse.Expressions;

namespace Pulse.Libraries
{

    /// <summary>
    /// File support library
    /// </summary>
    //public class LibraryFile : Library
    //{

    //    public const string A_DELETE = "DELETE";
    //    public const string A_COPY = "COPY";
    //    public const string A_MOVE = "MOVE";
    //    public const string A_WRITE_ALL = "WRITE_ALL";
    //    public const string A_OPEN_READ = "OPEN_READ";
    //    public const string A_OPEN_WRITE = "OPEN_WRITE";
    //    public const string A_CLOSE = "CLOSE";
    //    public const string A_WRITE_LINE = "WRITE_LINE";
    //    public const string A_DOWNLOAD = "DOWNLOAD";
    //    public const string A_ZIP = "ZIP";
    //    public const string A_UNZIP = "UNZIP";

    //    public const string F_SIZE = "SIZE";
    //    public const string F_EXISTS = "EXISTS";
    //    public const string F_EOF = "EOF";
    //    public const string F_READ_LINE = "READ_LINE";
    //    public const string F_READ_ALL_TEXT = "READ_ALL_TEXT";
    //    public const string S_MD5 = "MD5";

    //    private Heap<StreamReader> _TextReadStreams;
    //    private Heap<StreamWriter> _TextWriteStreams;

    //    public LibraryFile(Host Host)
    //        : base(Host, "FILE")
    //    {

    //        // Set up the streams //
    //        this._TextReadStreams = new Heap<StreamReader>();
    //        this._TextWriteStreams = new Heap<StreamWriter>();

    //    }

    //    /// <summary>
    //    /// Releases all streams
    //    /// </summary>
    //    public override void ShutDown()
    //    {

    //        foreach (StreamReader sr in this._TextReadStreams.Values)
    //        {
    //            sr.Close();
    //        }
    //        foreach (StreamWriter sw in this._TextWriteStreams.Values)
    //        {
    //            sw.Flush();
    //            sw.Close();
    //        }

    //    }

    //    /// <summary>
    //    /// A collection of all open text readers
    //    /// </summary>
    //    public Heap<StreamReader> TextReaders
    //    {
    //        get { return this._TextReadStreams; }
    //    }

    //    /// <summary>
    //    /// A collection of all open text writers
    //    /// </summary>
    //    public Heap<StreamWriter> TextWriters
    //    {
    //        get { return this._TextWriteStreams; }
    //    }

    //    public override ActionExpressionParameterized ActionLookup(string Name)
    //    {

    //        string n = Name.Trim().ToUpper();

    //        switch (n)
    //        {
    //            case A_DELETE:
    //                return new ActionExpressionDelete(this._Host, null);
    //            case A_COPY:
    //                return new ActionExpressionCopy(this._Host, null);
    //            case A_MOVE:
    //                return new ActionExpressionMove(this._Host, null);
    //            case A_WRITE_ALL:
    //                return new ActionExpressionWrite(this._Host, null);
    //            case A_OPEN_READ:
    //                return new ActionExpressionOpenRead(this._Host, null, this._TextReadStreams);
    //            case A_OPEN_WRITE:
    //                return new ActionExpressionOpenWrite(this._Host, null, this._TextWriteStreams);
    //            case A_CLOSE:
    //                return new ActionExpressionClose(this._Host, null, this._TextReadStreams, this._TextWriteStreams);
    //            case A_WRITE_LINE:
    //                return new ActionExpressionWriteLine(this._Host, null, this._TextWriteStreams);
    //            case A_DOWNLOAD:
    //                return new ActionExpressionDownload(this._Host, null);
    //            case A_ZIP:
    //                return new ActionExpressionZip(this._Host, null);
    //            case A_UNZIP:
    //                return new ActionExpressionUnzip(this._Host, null);
    //        }

    //        return null;

    //    }

    //    public override bool ActionExists(string Name)
    //    {
    //        return this.ActionLookup(Name) != null;
    //    }

    //    public override ScalarExpressionFunction ScalarFunctionLookup(string Name)
    //    {

    //        string t = Name.ToUpper().Trim();

    //        switch (t)
    //        {

    //            case F_EOF:
    //                return new ScalarExpressionEOF(this._TextReadStreams);
    //            case F_EXISTS:
    //                return new ScalarExpressionExists();
    //            case F_READ_LINE:
    //                return new ScalarExpressionReadLine(this._TextReadStreams);
    //            case F_SIZE:
    //                return new ScalarExpressionSize();
    //            case F_READ_ALL_TEXT:
    //                return new ScalarExpressionReadAllText();
    //            case S_MD5:
    //                return new ScalarExpressionMD5(this._Host);

    //        }

    //        return null;

    //    }

    //    public override bool ScalarFunctionExists(string Name)
    //    {
    //        return this.ScalarFunctionLookup(Name) != null;
    //    }

    //    // Actions //
    //    public class ActionExpressionDelete : ActionExpressionParameterized
    //    {

    //        public ActionExpressionDelete(Host Host, ActionExpression Parent)
    //            : base(Host, Parent, A_DELETE, "PATH.S.R", "Deletes a give file")
    //        {
    //        }

    //        public override void Invoke(FieldResolver Variant)
    //        {

    //            this.CheckRequired();

    //            string path = this._Parameters[0].Scalar.Evaluate(Variant).valueSTRING;

    //            if (File.Exists(path))
    //                File.Delete(path);

    //        }


    //    }

    //    public class ActionExpressionCopy : ActionExpressionParameterized
    //    {

    //        public ActionExpressionCopy(Host Host, ActionExpression Parent)
    //            : base(Host, Parent, A_COPY, "OLD_PATH.S.R;NEW_PATH.S.R", "Copies a given file")
    //        {
    //        }

    //        public override void Invoke(FieldResolver Variant)
    //        {

    //            this.CheckRequired();

    //            string old_path = this._Parameters[0].Scalar.Evaluate(Variant).valueSTRING;
    //            string new_path = this._Parameters[1].Scalar.Evaluate(Variant).valueSTRING;

    //            if (File.Exists(new_path))
    //                File.Delete(new_path);
    //            File.Copy(old_path, new_path);

    //        }


    //    }

    //    public class ActionExpressionMove : ActionExpressionParameterized
    //    {

    //        public ActionExpressionMove(Host Host, ActionExpression Parent)
    //            : base(Host, Parent, A_MOVE, "OLD_PATH.S.R;NEW_PATH.S.R", "Moves a given file")
    //        {
    //        }

    //        public override void Invoke(FieldResolver Variant)
    //        {

    //            this.CheckRequired();

    //            string old_path = this._Parameters[0].Scalar.Evaluate(Variant).valueSTRING;
    //            string new_path = this._Parameters[1].Scalar.Evaluate(Variant).valueSTRING;

    //            if (File.Exists(new_path))
    //                File.Delete(new_path);
    //            File.Move(old_path, new_path);

    //        }


    //    }

    //    public class ActionExpressionWrite : ActionExpressionParameterized
    //    {

    //        public ActionExpressionWrite(Host Host, ActionExpression Parent)
    //            : base(Host, Parent, A_WRITE_ALL, "PATH.S.R;VALUE.S.R", "Writes data to the end of an existing file; if the file does not exist, it will be created")
    //        {
    //        }

    //        public override void Invoke(FieldResolver Variant)
    //        {

    //            this.CheckRequired();

    //            string path = this._Parameters[0].Scalar.Evaluate(Variant).valueSTRING;
    //            Cell value = this._Parameters[1].Scalar.Evaluate(Variant);

    //            if (File.Exists(path))
    //            {
    //                if (value.Affinity == CellAffinity.STRING)
    //                {
    //                    File.AppendAllText(path, value.valueSTRING);
    //                }
    //                else
    //                {
    //                    byte[] b = value.valueBLOB;
    //                    using (FileStream fs = File.OpenWrite(path))
    //                    {
    //                        fs.Write(b, 0, b.Length);
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                if (value.Affinity == CellAffinity.STRING)
    //                {
    //                    File.WriteAllText(path, value.valueSTRING);
    //                }
    //                else
    //                {
    //                    File.WriteAllBytes(path, value.valueBLOB);
    //                }
    //            }

    //        }

    //    }

    //    public class ActionExpressionOpenRead : ActionExpressionParameterized
    //    {

    //        private Heap<StreamReader> _Streams;

    //        public ActionExpressionOpenRead(Host Host, ActionExpression Parent, Heap<StreamReader> Streams)
    //            : base(Host, Parent, A_OPEN_READ, "PATH.S.R;ALIAS.S.R", "Opens a file for reading; the file must exist")
    //        {
    //            this._Streams = Streams;
    //        }

    //        public override void Invoke(FieldResolver Variant)
    //        {

    //            this.CheckRequired();

    //            string path = this._Parameters["PATH"].Scalar.Evaluate(Variant).valueSTRING;
    //            string alias = this._Parameters["ALIAS"].Scalar.Evaluate(Variant).valueSTRING;

    //            if (!File.Exists(path))
    //                throw new Exception(string.Format("File '{0}' does not exist", path));

    //            this._Streams.Reallocate(alias, new StreamReader(path));

    //        }


    //    }

    //    public class ActionExpressionOpenWrite : ActionExpressionParameterized
    //    {

    //        private Heap<StreamWriter> _Streams;

    //        public ActionExpressionOpenWrite(Host Host, ActionExpression Parent, Heap<StreamWriter> Streams)
    //            : base(Host, Parent, A_OPEN_WRITE, "PATH.S.R;ALIAS.S.R", "Opens a file for writing; if the file doesn't exist, it will be created")
    //        {
    //            this._Streams = Streams;
    //        }

    //        public override void Invoke(FieldResolver Variant)
    //        {

    //            this.CheckRequired();

    //            string path = this._Parameters["PATH"].Scalar.Evaluate(Variant).valueSTRING;
    //            string alias = this._Parameters["ALIAS"].Scalar.Evaluate(Variant).valueSTRING;

    //            if (!File.Exists(path))
    //            {
    //                FileStream fs = File.Create(path);
    //                this._Streams.Reallocate(alias, new StreamWriter(fs));
    //                return;
    //            }

    //            this._Streams.Reallocate(alias, new StreamWriter(path));

    //        }


    //    }

    //    public class ActionExpressionClose : ActionExpressionParameterized
    //    {

    //        private Heap<StreamReader> _RStreams;
    //        private Heap<StreamWriter> _WStreams;

    //        public ActionExpressionClose(Host Host, ActionExpression Parent, Heap<StreamReader> RStreams, Heap<StreamWriter> WStreams)
    //            : base(Host, Parent, A_CLOSE, "ALIAS.S.R", "Closes an open stream; if both a read and write stream have the same, both will be closed")
    //        {
    //            this._RStreams = RStreams;
    //            this._WStreams = WStreams;
    //        }

    //        public override void Invoke(FieldResolver Variant)
    //        {

    //            this.CheckRequired();

    //            string alias = this._Parameters["ALIAS"].Scalar.Evaluate(Variant).valueSTRING;

    //            if (this._RStreams.Exists(alias))
    //            {
    //                StreamReader sr = this._RStreams[alias];
    //                sr.Close();
    //                this._RStreams.Deallocate(alias);
    //            }

    //            if (this._WStreams.Exists(alias))
    //            {
    //                StreamWriter sw = this._WStreams[alias];
    //                sw.Close();
    //                sw.Flush();
    //                this._WStreams.Deallocate(alias);
    //            }

    //        }


    //    }

    //    public class ActionExpressionWriteLine : ActionExpressionParameterized
    //    {

    //        private Heap<StreamWriter> _Streams;

    //        public ActionExpressionWriteLine(Host Host, ActionExpression Parent, Heap<StreamWriter> Streams)
    //            : base(Host, Parent, A_CLOSE, "ALIAS.S.R;VALUE.S.R", "Closes an open stream; if both a read and write stream have the same, both will be closed")
    //        {
    //            this._Streams = Streams;
    //        }

    //        public override void Invoke(FieldResolver Variant)
    //        {

    //            this.CheckRequired();

    //            string alias = this._Parameters["ALIAS"].Scalar.Evaluate(Variant).valueSTRING;
    //            string value = this._Parameters["ALIAS"].Scalar.Evaluate(Variant).valueSTRING;

    //            if (this._Streams.Exists(alias))
    //            {
    //                this._Streams[alias].WriteLine(value);
    //                return;
    //            }

    //            throw new Exception(string.Format("File '{0}' is not open", alias));

    //        }


    //    }

    //    public class ActionExpressionDownload : ActionExpressionParameterized
    //    {

    //        public ActionExpressionDownload(Host Host, ActionExpression Parent)
    //            : base(Host, Parent, A_DOWNLOAD, "PATH.S.R;URI.S.R;MESSAGE.S.O", "Downloads a given file")
    //        {
    //        }

    //        public override void Invoke(FieldResolver Variant)
    //        {

    //            this.CheckRequired();

    //            string path = this._Parameters[0].Scalar.Evaluate(Variant).valueSTRING;
    //            string uri = this._Parameters[1].Scalar.Evaluate(Variant).valueSTRING;
    //            string message = (this._Parameters.Count > 2 ? this._Parameters[2].Scalar.Evaluate(Variant).valueSTRING : null);

    //            // Download the data
    //            try
    //            {

    //                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(uri);
    //                req.CookieContainer = new CookieContainer();
    //                req.Method = "GET";
    //                using (WebResponse res = req.GetResponse())
    //                {

    //                    using (Stream s = res.GetResponseStream())
    //                    {

    //                        string html = "";
    //                        using (StreamReader sr = new StreamReader(s))
    //                        {
    //                            html = sr.ReadToEnd();
    //                        }

    //                        using (StreamWriter sw = new StreamWriter(path))
    //                        {
    //                            sw.Write(html);
    //                        }

    //                    }

    //                }

    //                // Message //
    //                if (message != null)
    //                {
    //                    this._Host.IO.WriteLine(string.Format("File.Download : Sucess : {0}", message));
    //                }


    //            }
    //            catch (Exception e)
    //            {

    //                // Message //
    //                if (message != null)
    //                {
    //                    this._Host.IO.WriteLine(string.Format("File.Download : Fail : {0}", message));
    //                }

    //            }

                


    //        }

    //    }

    //    public class ActionExpressionZip : ActionExpressionParameterized
    //    {

    //        public ActionExpressionZip(Host Host, ActionExpression Parent)
    //            : base(Host, Parent, A_ZIP, "IN_PATH.S.R;OUT_PATH.S.R", "Zips a given file or directory")
    //        {
    //        }

    //        public override void Invoke(FieldResolver Variant)
    //        {

    //            this.CheckRequired();

    //            string in_path = this._Parameters[0].Scalar.Evaluate(Variant).valueSTRING;
    //            string out_path = this._Parameters[1].Scalar.Evaluate(Variant).valueSTRING;

    //            System.IO.Compression.ZipFile.CreateFromDirectory(in_path, out_path);

    //        }

    //    }

    //    public class ActionExpressionUnzip : ActionExpressionParameterized
    //    {

    //        public ActionExpressionUnzip(Host Host, ActionExpression Parent)
    //            : base(Host, Parent, A_UNZIP, "IN_PATH.S.R;OUT_PATH.S.R", "Unzips a given file or directory")
    //        {
    //        }

    //        public override void Invoke(FieldResolver Variant)
    //        {

    //            this.CheckRequired();

    //            string in_path = this._Parameters[0].Scalar.Evaluate(Variant).valueSTRING;
    //            string out_path = this._Parameters[1].Scalar.Evaluate(Variant).valueSTRING;

    //            Console.WriteLine(in_path);
    //            Console.WriteLine(out_path);

    //            System.IO.Compression.ZipFile.ExtractToDirectory(in_path, out_path);

    //        }

    //    }

    //    // Functions //
    //    public class ScalarExpressionSize : ScalarExpressionFunction
    //    {

    //        public ScalarExpressionSize()
    //            : base(null, F_SIZE, 1)
    //        {
    //        }

    //        public override CellAffinity ReturnAffinity()
    //        {
    //            return CellAffinity.LONG;
    //        }

    //        public override ScalarExpression CloneOfMe()
    //        {
    //            return new ScalarExpressionSize();
    //        }

    //        public override Cell Evaluate(FieldResolver Variants)
    //        {
    //            string path = this._ChildNodes[0].Evaluate(Variants).valueSTRING;
    //            if (!File.Exists(path))
    //                return CellValues.NullLONG;
    //            return new Cell(new FileInfo(path).Length);
    //        }

    //    }

    //    public class ScalarExpressionExists : ScalarExpressionFunction
    //    {

    //        public ScalarExpressionExists()
    //            : base(null, F_EXISTS, 1)
    //        {
    //        }

    //        public override CellAffinity ReturnAffinity()
    //        {
    //            return CellAffinity.BOOL;
    //        }

    //        public override ScalarExpression CloneOfMe()
    //        {
    //            return new ScalarExpressionExists();
    //        }

    //        public override Cell Evaluate(FieldResolver Variants)
    //        {
    //            string path = this._ChildNodes[0].Evaluate(Variants).valueSTRING;
    //            if (File.Exists(path))
    //                return CellValues.True;
    //            return CellValues.False;
    //        }


    //    }

    //    public class ScalarExpressionEOF : ScalarExpressionFunction
    //    {

    //        private Heap<StreamReader> _Reader;

    //        public ScalarExpressionEOF(Heap<StreamReader> Reader)
    //            : base(null, F_EOF, 1)
    //        {
    //            this._Reader = Reader;
    //        }

    //        public override CellAffinity ReturnAffinity()
    //        {
    //            return CellAffinity.BOOL;
    //        }

    //        public override ScalarExpression CloneOfMe()
    //        {
    //            return new ScalarExpressionEOF(this._Reader);
    //        }

    //        public override Cell Evaluate(FieldResolver Variants)
    //        {

    //            string path = this._ChildNodes[0].Evaluate(Variants).valueSTRING;
    //            if (!this._Reader.Exists(path))
    //                return CellValues.NullBOOL;
    //            return (this._Reader[path].EndOfStream ? CellValues.True : CellValues.False);

    //        }


    //    }

    //    public class ScalarExpressionReadLine : ScalarExpressionFunction
    //    {

    //        private Heap<StreamReader> _Reader;

    //        public ScalarExpressionReadLine(Heap<StreamReader> Reader)
    //            : base(null, F_READ_LINE, 1)
    //        {
    //            this._Reader = Reader;
    //        }

    //        public override CellAffinity ReturnAffinity()
    //        {
    //            return CellAffinity.STRING;
    //        }

    //        public override ScalarExpression CloneOfMe()
    //        {
    //            return new ScalarExpressionReadLine(this._Reader);
    //        }

    //        public override Cell Evaluate(FieldResolver Variants)
    //        {

    //            string path = this._ChildNodes[0].Evaluate(Variants).valueSTRING;
    //            if (!this._Reader.Exists(path))
    //                return CellValues.NullSTRING;
    //            return (!this._Reader[path].EndOfStream ? new Cell(this._Reader[path].ReadLine()) : CellValues.NullSTRING);

    //        }


    //    }

    //    private sealed class ScalarExpressionMD5 : ScalarExpressionFunction
    //    {

    //        private Host _Host;

    //        public ScalarExpressionMD5(Host Host)
    //            : base(null, S_MD5, 1)
    //        {
    //            this._Host = Host;
    //        }

    //        public override CellAffinity ReturnAffinity()
    //        {
    //            return CellAffinity.BLOB;
    //        }

    //        public override ScalarExpression CloneOfMe()
    //        {
    //            return new ScalarExpressionMD5(this._Host);
    //        }

    //        public override Cell Evaluate(FieldResolver Variants)
    //        {

    //            string value = this._ChildNodes[0].Evaluate(Variants).valueSTRING;
    //            byte[] b;
    //            using (System.IO.Stream river = System.IO.File.Open(value, System.IO.FileMode.Open, System.IO.FileAccess.Read))
    //            {

    //                using (System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider())
    //                {
    //                    b = md5.ComputeHash(river);
    //                }

    //            }

    //            return new Cell(b);

    //        }

    //    }

    //    public class ScalarExpressionReadAllText : ScalarExpressionFunction
    //    {

    //        public ScalarExpressionReadAllText()
    //            : base(null, F_READ_ALL_TEXT, 1)
    //        {
    //        }

    //        public override CellAffinity ReturnAffinity()
    //        {
    //            return CellAffinity.TEXT;
    //        }

    //        public override ScalarExpression CloneOfMe()
    //        {
    //            return new ScalarExpressionReadAllText();
    //        }

    //        public override Cell Evaluate(FieldResolver Variants)
    //        {

    //            string path = this._ChildNodes[0].Evaluate(Variants).valueSTRING;
    //            return new Cell(File.ReadAllText(path), true);

    //        }


    //    }


    //}



}
