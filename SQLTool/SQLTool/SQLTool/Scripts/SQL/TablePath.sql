--*****************************************************************************************************************
--To get the unique paths from It_Depends_Custom() (which was used to insert records in T_TEMP_PATH_CASE_TRF_INFO)
--*****************************************************************************************************************

/*

SELECT  TheFullEntityName , iteration, ThePath into #testdata1 from dbo.It_Depends_Custom()

DECLARE   @TableRanking TABLE (ThePath VARCHAR(max))
DECLARE  @TheFullEntityName varchar(500),  @iteration int , @path varchar(1000),  @ii int = 1, @maxii int;  

select  @maxii=max(iteration) from #testdata1

WHILE @maxii >= @ii  
begin
	DECLARE path_cursor CURSOR FOR   
	SELECT * from #testdata1 where iteration = @maxii 
  
	OPEN path_cursor 
  
	FETCH NEXT FROM path_cursor   
	INTO @TheFullEntityName, @iteration, @path  
	 
  
	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		if not exists(select ThePath from @TableRanking where ThePath like '%' + @path + '%' )
		begin
			insert into @TableRanking
			select @path
		end
		FETCH NEXT FROM path_cursor   
		INTO @TheFullEntityName, @iteration, @path  
	END   
	CLOSE path_cursor;  
	DEALLOCATE path_cursor;

	set @maxii-=1
end 
drop table #testdata1

SELECT * FROM @TableRanking

*/

SELECT  TheFullEntityName , iteration, ThePath from dbo.It_Depends_Custom()





