using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// C#のAttribute（属性）のテスト。
/// <summary>
/// クラスの作者を記録しておくための属性
/// </summary>
/// <seealso cref="http://ufcpp.net/study/csharp/sp_attribute.html#userdefine"/>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class AuthorAttribute : Attribute
{
	/// <summary>
	/// 作者名
	/// </summary>
	private string Name;
	/// <summary>
	/// 作者所属
	/// </summary>
	public string Affiliation;
	public AuthorAttribute(string name) { this.Name = name; }
}
