// 以下　http://blogs.msdn.com/b/ito/archive/2008/02/22/simple-japanese-message.aspx　より使わせて頂きました
using System;
using System.IO;

//　DisplayNameやDescriptionアトリビュートを使うのに必要
using System.ComponentModel;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace Pipeline
{
    /// <summary>
    /// 簡単なメッセージプロセッサ
    /// FontDescriptionProcessorから派生して、MessageFilenameという
    /// プロセッサパラメーターを追加してある。
    /// このパラメーターに指定されたファイル内の文字列を
    /// FontDescriptionへ追加することで、そのファイル内のメッセージを
    /// ゲーム中で使えるようにする
    /// </summary>
    [ContentProcessor(DisplayName = "メッセージ プロセッサ")]
    public class MessageProcessor : FontDescriptionProcessor
    {
        /// <summary>
        /// プロセッサーパラメーター
        /// ここに読み込むメッセージファイル名を指定する
        /// </summary>
        // プロパティ画面で表示される文字列の指定
        [DisplayName("メッセージファイル名")]
        // プロパティ画面でのパラメーターの説明
        [Description("メッセージテキストが含まれているテキストファイル名")]
        public string MessageFilename
        {
            get { return messageFilename; }
            set { messageFilename = value; }
        }

        string messageFilename;

        /// <summary>
        /// コンテントパイプラインから呼ばれるメソッド
        /// </summary>
        public override SpriteFontContent Process(FontDescription input,
                                                    ContentProcessorContext context)
        {
            // MessageFilenameで指定されたファイル内の文字を追加する 
            AppendCharacters(input, context);

            // 文字列を追加した後は単純にFontDescriptionProcessorのプロセスを呼ぶだけ
            return base.Process(input, context);
        }

        /// <summary>
        /// FontDescriptionにMessageFilenameで指定されたファイル内の文字を追加する
        /// </summary>
        void AppendCharacters(FontDescription input, ContentProcessorContext context)
        {
            // MessageFilenameは有効な文字列か？
            if (String.IsNullOrEmpty(MessageFilename))
                return;

            if (!File.Exists(MessageFilename))
            {
                throw new FileNotFoundException(
                    String.Format( "MessageFilenameで指定されたファイル[{0}]が存在しません",
                                    Path.GetFullPath(MessageFilename)));
            }

            // 指定されたファイルから文字列を読み込み、
            // FontDescription.Charctarsに追加する
            try
            {
                int totalCharacterCount = 0;

                using (StreamReader sr = File.OpenText(MessageFilename))
                {
                    string line;
                    while ( ( line = sr.ReadLine() ) != null )
                    {
                        totalCharacterCount += line.Length;

                        foreach( char c in line )
                            input.Characters.Add( c );
                    }
                }

                context.Logger.LogImportantMessage("使用文字数{0}, 総文字数:{1}",
                    input.Characters.Count, totalCharacterCount);

                // CPにファイル依存していることを教える
                context.AddDependency(Path.GetFullPath(MessageFilename));
            }
            catch (Exception e)
            {
                // 予期しない例外が発生
                context.Logger.LogImportantMessage("例外発生!! {0}", e.Message);
                throw e;
            }
        }
    }
}