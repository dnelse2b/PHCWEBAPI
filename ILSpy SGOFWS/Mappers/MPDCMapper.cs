using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.Configuration;
using SGOFWS.Domains.Models;
using SGOFWS.DTOs;
using SGOFWS.Extensions;
using SGOFWS.Helper;

namespace SGOFWS.Mappers;

public class MPDCMapper
{
	private readonly ConversionExtension conversionExtension = new ConversionExtension();

	private readonly GeoHelper geoHelper = new GeoHelper();

	public List<Bo3> mapConsignacao(List<UpdateConsignmentDataMPDCDTO> consignacao)
	{
		MapperConfiguration config = new MapperConfiguration(delegate(IMapperConfigurationExpression cfg)
		{
			cfg.CreateMap<UpdateConsignmentDataMPDCDTO, Bo3>().ForMember((Bo3 dest) => dest.UConsgt, delegate(IMemberConfigurationExpression<UpdateConsignmentDataMPDCDTO, Bo3, string> opt)
			{
				opt.MapFrom((UpdateConsignmentDataMPDCDTO src) => src.consignmentNumber);
			}).ForMember((Bo3 dest) => dest.UAgnuit, delegate(IMemberConfigurationExpression<UpdateConsignmentDataMPDCDTO, Bo3, string> opt)
			{
				opt.MapFrom((UpdateConsignmentDataMPDCDTO src) => src.agentNuit);
			})
				.ForMember((Bo3 dest) => dest.UAgnome, delegate(IMemberConfigurationExpression<UpdateConsignmentDataMPDCDTO, Bo3, string> opt)
				{
					opt.MapFrom((UpdateConsignmentDataMPDCDTO src) => src.customerName);
				});
		});
		IMapper mapper = config.CreateMapper();
		return mapper.Map<List<Bo3>>(consignacao);
	}

	public Dossier mapDossier(UpdateConsignmentDataMPDCDTO consignacaoTFRDTO)
	{
		MapperConfiguration config = new MapperConfiguration(delegate(IMapperConfigurationExpression cfg)
		{
			cfg.CreateMap<UpdateConsignmentDataMPDCDTO, Dossier>().ForPath((Dossier dest) => dest.bo2.Bo2stamp, delegate(IPathConfigurationExpression<UpdateConsignmentDataMPDCDTO, Dossier, string> act)
			{
				act.MapFrom((UpdateConsignmentDataMPDCDTO src) => "stamp");
			}).ForPath((Dossier dest) => dest.bo3.UConsgno, delegate(IPathConfigurationExpression<UpdateConsignmentDataMPDCDTO, Dossier, string> act)
			{
				act.MapFrom((UpdateConsignmentDataMPDCDTO src) => conversionExtension.nullToString(src.consignmentNumber));
			})
				.ForPath((Dossier dest) => dest.bo.No, delegate(IPathConfigurationExpression<UpdateConsignmentDataMPDCDTO, Dossier, decimal?> act)
				{
					act.MapFrom((UpdateConsignmentDataMPDCDTO src) => 0);
				})
				.ForPath((Dossier dest) => dest.bo.Nome, delegate(IPathConfigurationExpression<UpdateConsignmentDataMPDCDTO, Dossier, string> act)
				{
					act.MapFrom((UpdateConsignmentDataMPDCDTO src) => "");
				})
				.ForPath((Dossier dest) => dest.bi, delegate(IPathConfigurationExpression<UpdateConsignmentDataMPDCDTO, Dossier, List<Bi>> o)
				{
					o.MapFrom((UpdateConsignmentDataMPDCDTO src) => src.wagons.Select((WagonMPDCDTO wg) => new Bi
					{
						Nome = "",
						No = 0m
					}));
				})
				.ForPath((Dossier dest) => dest.bi2, delegate(IPathConfigurationExpression<UpdateConsignmentDataMPDCDTO, Dossier, List<Bi2>> o)
				{
					o.MapFrom((UpdateConsignmentDataMPDCDTO src) => src.wagons.Select((WagonMPDCDTO wg) => new Bi2
					{
						UVagno = conversionExtension.nullToString(wg.wagonNumber),
						UAdmintr = geoHelper.getAdminByCountryCode(conversionExtension.nullToString(src.countryCode)),
						UDsccmcag = conversionExtension.nullToString(wg.commodity),
						UProxst = "Chicualacuala",
						UPeso = conversionExtension.toTons(conversionExtension.nullToString(wg.weight)),
						UFullempy = ((conversionExtension.nullToString(wg.wagonStatus) == "loaded") ? true : false),
						UAgemail = conversionExtension.nullToString(src.agentEmail),
						UAgnome = conversionExtension.nullToString(src.agentName),
						UAgtelef = conversionExtension.nullToString(src.agentPhone),
						UAgnuit = conversionExtension.nullToString(src.agentNuit)
					}));
				});
		});
		Mapper mapper = new Mapper(config);
		return mapper.Map<Dossier>(consignacaoTFRDTO);
	}
}
