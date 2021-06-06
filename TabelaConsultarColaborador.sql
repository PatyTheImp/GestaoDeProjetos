SELECT  t.nome, t.area, t.custo
FROM TecnologiaColaborador AS tc
JOIN Colaborador AS c
	ON c.nif = tc.colaborador_nif
JOIN Tecnologia AS t
	ON t.id = tc.tecnologia_id
WHERE c.nif = 123456789;