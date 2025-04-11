<UserControl
    x:Class="{Namespace}.{ClassName}"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:i="http://schemas.microsoft.com/xaml/behaviors"
    xmlns:local="clr-namespace:{Namespace}"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    xmlns:uc="clr-namespace:{Namespace}"
    d:DesignHeight="450"
    d:DesignWidth="800"
    Background="Blue"
    mc:Ignorable="d">
    
    <!-- <Grid DataContext="{Binding Source={{StaticResource VML}}, Path={ViewModelBinding}}"> -->
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="50" />
            <RowDefinition Height="*" />
        </Grid.RowDefinitions>

        <Border Grid.Row="0" Background="LightBlue" BorderBrush="red" BorderThickness="2">
            <Grid>
                <StackPanel Margin="10,0,0,0" HorizontalAlignment="Left" VerticalAlignment="Center" Orientation="Horizontal">                
                    <TextBlock  Text="{Title}" />
                </StackPanel>
                 <!-- <uc:ucWerkBalk HorizontalAlignment="Right" VerticalAlignment="Top" /> -->
            </Grid>
        </Border>

        <Border Grid.Row="1" Margin="3" BorderBrush="Black" BorderThickness="1" CornerRadius="5">
            <Grid>
                <!-- Je kan hier extra inhoud injecteren per view -->
            </Grid>
        </Border>
    </Grid>
</UserControl>
