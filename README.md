#Introduction
This is a pull to refresh listview for uwp(universal windows platform)

You can use this listview with the follow codes:

<font color=Blue><pre><code>
	&lt;local:PullToRefreshListView x:Name="lv" 
		PullPartTemplate="Pull" ReleasePartTemplate="Release" 
		RefreshContent="lv_RefreshContent" MoreContent="lv_MoreContent"/&gt;
</code></pre></font>
<font color=Blue><pre><code>
	private async void lv_RefreshContent(object sender, EventArgs e)
	{
		progressBar.Visibility = Visibility.Visible;
		await Task.Delay(2000);
		for (int i = 0; i < 10; i++)
		{
			Data.Insert(0, "New One");
		}
		progressBar.Visibility = Visibility.Collapsed;
	}
	
	private async void lv_MoreContent(object sender, EventArgs e)
	{
		progressBar.Visibility = Visibility.Visible;
		await Task.Delay(2000);
		for (int i = 0; i < 10; i++)
		{
			Data.Add("Old One");
		}
		progressBar.Visibility = Visibility.Collapsed;
	}
</code></pre></font>
For more information, to see the SamplePage in source.