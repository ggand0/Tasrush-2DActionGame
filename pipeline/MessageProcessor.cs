// �ȉ��@http://blogs.msdn.com/b/ito/archive/2008/02/22/simple-japanese-message.aspx�@���g�킹�Ē����܂���
using System;
using System.IO;

//�@DisplayName��Description�A�g���r���[�g���g���̂ɕK�v
using System.ComponentModel;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace Pipeline
{
    /// <summary>
    /// �ȒP�ȃ��b�Z�[�W�v���Z�b�T
    /// FontDescriptionProcessor����h�����āAMessageFilename�Ƃ���
    /// �v���Z�b�T�p�����[�^�[��ǉ����Ă���B
    /// ���̃p�����[�^�[�Ɏw�肳�ꂽ�t�@�C�����̕������
    /// FontDescription�֒ǉ����邱�ƂŁA���̃t�@�C�����̃��b�Z�[�W��
    /// �Q�[�����Ŏg����悤�ɂ���
    /// </summary>
    [ContentProcessor(DisplayName = "���b�Z�[�W �v���Z�b�T")]
    public class MessageProcessor : FontDescriptionProcessor
    {
        /// <summary>
        /// �v���Z�b�T�[�p�����[�^�[
        /// �����ɓǂݍ��ރ��b�Z�[�W�t�@�C�������w�肷��
        /// </summary>
        // �v���p�e�B��ʂŕ\������镶����̎w��
        [DisplayName("���b�Z�[�W�t�@�C����")]
        // �v���p�e�B��ʂł̃p�����[�^�[�̐���
        [Description("���b�Z�[�W�e�L�X�g���܂܂�Ă���e�L�X�g�t�@�C����")]
        public string MessageFilename
        {
            get { return messageFilename; }
            set { messageFilename = value; }
        }

        string messageFilename;

        /// <summary>
        /// �R���e���g�p�C�v���C������Ă΂�郁�\�b�h
        /// </summary>
        public override SpriteFontContent Process(FontDescription input,
                                                    ContentProcessorContext context)
        {
            // MessageFilename�Ŏw�肳�ꂽ�t�@�C�����̕�����ǉ����� 
            AppendCharacters(input, context);

            // �������ǉ�������͒P����FontDescriptionProcessor�̃v���Z�X���ĂԂ���
            return base.Process(input, context);
        }

        /// <summary>
        /// FontDescription��MessageFilename�Ŏw�肳�ꂽ�t�@�C�����̕�����ǉ�����
        /// </summary>
        void AppendCharacters(FontDescription input, ContentProcessorContext context)
        {
            // MessageFilename�͗L���ȕ����񂩁H
            if (String.IsNullOrEmpty(MessageFilename))
                return;

            if (!File.Exists(MessageFilename))
            {
                throw new FileNotFoundException(
                    String.Format( "MessageFilename�Ŏw�肳�ꂽ�t�@�C��[{0}]�����݂��܂���",
                                    Path.GetFullPath(MessageFilename)));
            }

            // �w�肳�ꂽ�t�@�C�����當�����ǂݍ��݁A
            // FontDescription.Charctars�ɒǉ�����
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

                context.Logger.LogImportantMessage("�g�p������{0}, ��������:{1}",
                    input.Characters.Count, totalCharacterCount);

                // CP�Ƀt�@�C���ˑ����Ă��邱�Ƃ�������
                context.AddDependency(Path.GetFullPath(MessageFilename));
            }
            catch (Exception e)
            {
                // �\�����Ȃ���O������
                context.Logger.LogImportantMessage("��O����!! {0}", e.Message);
                throw e;
            }
        }
    }
}