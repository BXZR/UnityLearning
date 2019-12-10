function table_2_string(t)
	local printed_table_cache = {}
	local result_string = ""
	local CRLF = '\n'
	local function table_2_string_core(t, indent) 
		if (printed_table_cache[tostring(t)]) then
            result_string = result_string..indent.."*"..tostring(t)..CRLF
        else
            printed_table_cache[tostring(t)]=true
            if (type(t)=="table") then
                for pos,val in pairs(t) do
                    if (type(val)=="table") then
                        result_string = result_string..indent.."["..pos.."] => "..tostring(t).." {"..CRLF
                        table_2_string_core(val,indent..string.rep(" ",string.len(pos)+8))
                        result_string = result_string..indent..string.rep(" ",string.len(pos)+6).."}"..CRLF
                    elseif (type(val)=="string") then
                        result_string = result_string..indent.."["..pos..'] => "'..val..'"'..CRLF
                    else
                        result_string = result_string..indent.."["..pos.."] => "..tostring(val)..CRLF
                    end
                end
            else
                result_string = result_string..indent..tostring(t)..CRLF
            end
        end
    end
    if (type(t)=="table") then
        result_string = result_string..tostring(t).." {"..CRLF
        table_2_string_core(t,"  ")
        result_string = result_string.."}"..CRLF
    else
        table_2_string_core(t,"  ")
    end
    result_string = result_string..CRLF
    return result_string
end


x = {}
x.z = 22
x.cc = 45456

ccz = table_2_string(x)
print(ccz)